import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output, ViewEncapsulation, computed, inject } from '@angular/core';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { Router, RouterModule, IsActiveMatchOptions } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../../../core/service/auth/auth.service';
import { PermissionService } from '../../../core/service/permission/permission.service';
import { SweetAlertService } from '../../Services/sweet-alert/sweet-alert.service';
import { BackendMenuItem, SidebarItem } from './sidebar.config';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  encapsulation: ViewEncapsulation.None,
  imports: [
    CommonModule,
    RouterModule,
    MatListModule,
    MatIconModule,
    MatExpansionModule,
  ],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css'],
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

  cerrarSesion(): void {
    this.sweetAlertService
      .showConfirm('¿Estás seguro?', 'Tu sesión actual se cerrará.', 'Sí, salir', 'Cancelar')
      .then((result) => {
        if (result.isConfirmed) {
          this.sweetAlertService.showLoading('Cerrando sesión', 'Por favor, espera...');
          this.authService.logout()
            .pipe(finalize(() => this.sweetAlertService.hideLoading()))
            .subscribe({
              error: (err) => {
                this.sweetAlertService.showNotification(
                  'Error',
                  err?.error?.message ?? 'Ocurrió un error al cerrar la sesión.',
                  'error'
                );
              },
            });
        }
      });
  }

  private transformMenu(backendMenu: readonly BackendMenuItem[] | null): SidebarItem[] {
    if (!backendMenu) return [];

    const sortedMenu = [...backendMenu].sort((a, b) => a.id - b.id);

    return sortedMenu.flatMap<SidebarItem>((module) => {
      const forms = [...(module.forms ?? [])]
        .sort((a, b) => a.id - b.id)
        .filter((f, i, arr) => arr.findIndex(x => x.route === f.route) === i);

      if (forms.length === 0) return [];

      if (forms.length === 1) {
        const only = forms[0];
        return [{
          label: module.name,
          icon: module.icon,
          route: `/admin/${only.route}`,
          children: [],
        }];
      }

      return [{
        label: module.name,
        icon: module.icon,
        children: forms.map(f => ({
          label: f.name,
          icon: '',
          route: `/admin/${f.route}`,
        })),
      }];
    });
  }

  isGroupActive(item: SidebarItem): boolean {
    return (item.children ?? []).some(c => this.router.isActive(c.route, this.exactMatch));
  }

  isSingleActive(item: SidebarItem): boolean {
    return !!item.route && this.router.isActive(item.route, this.exactMatch);
  }

  trackByItem = (_: number, item: SidebarItem) => item.label;
  trackByChild = (_: number, child: { route: string }) => child.route;
}
