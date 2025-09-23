import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Output,
  computed,
  inject,
} from '@angular/core';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { Router, RouterModule, IsActiveMatchOptions } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../../../core/security/services/auth/auth.service';
import { PermissionService } from '../../../core/security/services/permission/permission.service';
import { SweetAlertService } from '../../Services/sweet-alert/sweet-alert.service';
import { BackendMenuItem, BackendSubMenuItem, SidebarItem, SidebarNode } from './sidebar.config';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule, MatListModule, MatIconModule, MatExpansionModule],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SidebarComponent {
  private readonly permissionService = inject(PermissionService);
  private readonly authService = inject(AuthService);
  private readonly sweetAlertService = inject(SweetAlertService);
  private readonly router = inject(Router);

  @Output() closeSidebar = new EventEmitter<void>();

  readonly menu = computed(() => {
    const backendMenu = this.permissionService.menu();
    return this.transformMenu(backendMenu);
  });

  private readonly exactMatch: IsActiveMatchOptions = {
    paths: 'exact',
    queryParams: 'ignored',
    matrixParams: 'ignored',
    fragment: 'ignored',
  };

  onItemClick(): void {
    if (window.innerWidth <= 768) this.closeSidebar.emit();
  }

  cerrarSesion(): void {
    this.sweetAlertService
      .showConfirm('¿Estás seguro?', 'Tu sesión actual se cerrará.', 'Sí, salir', 'Cancelar')
      .then((result) => {
        if (!result.isConfirmed) return;
        this.sweetAlertService.showLoading('Cerrando sesión', 'Por favor, espera...');
        this.authService
          .logout()
          .pipe(finalize(() => this.sweetAlertService.hideLoading()))
          .subscribe({
            error: (err) =>
              this.sweetAlertService.showNotification(
                'Error',
                err?.error?.message ?? 'Ocurrió un error al cerrar la sesión.',
                'error'
              ),
          });
      });
  }

  // Menú: transforma módulo -> árbol (hasta 3 niveles)
  private transformMenu(backendMenu: readonly BackendMenuItem[] | null): SidebarItem[] {
    if (!backendMenu) return [];

    // Orden estable por id de módulo
    const modules = [...backendMenu].sort((a, b) => a.id - b.id);

    const normalizeRoute = (route?: string | null): string | undefined => {
      if (!route) return undefined;
      const trimmed = route.trim();
      if (!trimmed) return undefined;

      let normalizedRoute = trimmed;
      while (normalizedRoute.startsWith('/')) {
        normalizedRoute = normalizedRoute.slice(1);
      }

      if (normalizedRoute.toLowerCase().startsWith('admin/')) {
        normalizedRoute = normalizedRoute.slice('admin/'.length);
      }

      if (!normalizedRoute) return undefined;

      return `/${normalizedRoute}`;
    };

    return modules.flatMap<SidebarItem>((mod) => {
      const forms = dedupeByRoute([...mod.forms].sort((a, b) => a.id - b.id));

      if (forms.length === 0) return [];

      // 1) Construimos árbol por parentId (scope: dentro del módulo)
      const nodesById = new Map<number, SidebarNode>();
      forms.forEach(f =>
        nodesById.set(f.id, {
          id: f.id,
          label: f.name,
          route: normalizeRoute(f.route),
          children: [],
        })
      );

      const roots: SidebarNode[] = [];
      for (const f of forms) {
        const node = nodesById.get(f.id)!;
        if (f.parentId && nodesById.has(f.parentId)) {
          nodesById.get(f.parentId)!.children.push(node);
        } else {
          roots.push(node);
        }
      }

      // 2) Normalizamos:
      // - Si hay 1 root y no tiene hijos => ítem directo (link)
      // - En caso contrario => acordeón con children (roots)
      if (roots.length === 1 && roots[0].children.length === 0) {
        // Ítem directo: módulo con único form simple
        const only = roots[0];
        return [{
          label: mod.name,
          icon: mod.icon,
          route: normalizeRoute(only.route),
          children: [],
        }];
      }

      // Acordeón: módulo con 1..n roots (cada root puede tener hijos)
      return [{
        label: mod.name,
        icon: mod.icon,
        children: roots.map(r => normalizeRoutes(r)), // asegura que los leaves tengan route
      }];
    });

    // Helpers locales
    function dedupeByRoute(arr: BackendSubMenuItem[]): BackendSubMenuItem[] {
      const seen = new Set<string>();
      return arr.filter(x => {
        const key = normalizeRoute(x.route) ?? `__no_route__${x.id}`;
        if (seen.has(key)) return false;
        seen.add(key);
        return true;
      });
    }

    // Nodo root sin route: el template lo muestra como título (no clickeable)
    function normalizeRoutes(n: SidebarNode): SidebarNode {
      const sanitizedRoute = normalizeRoute(n.route);
      return {
        ...n,
        route: sanitizedRoute,
        children: (n.children ?? []).map(c => normalizeRoutes(c)),
      };
    }
  }

  // Active states
  isGroupActive(item: SidebarItem): boolean {
    // activo si alguno de sus hijos (o subhijos) está activo
    return (item.children ?? []).some(c => this.isNodeActive(c));
  }

  private isNodeActive(node: SidebarNode): boolean {
    if (node.route) {
      const target = node.route.startsWith('/') ? node.route : `/${node.route}`;
      if (this.router.isActive(target, this.exactMatch)) return true;
    }
    return (node.children ?? []).some(ch => this.isNodeActive(ch));
  }

  isSingleActive(item: SidebarItem): boolean {
    return !!item.route && this.router.isActive(item.route, this.exactMatch);
  }

  // trackBys
  trackByItem = (_: number, item: SidebarItem) => item.label;
  trackByChild = (_: number, child: SidebarNode) => String(child.id ?? child.label);
  trackByGrand = (_: number, child: SidebarNode) => String(child.id ?? child.label);
}
