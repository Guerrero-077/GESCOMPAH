import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output, computed, inject } from '@angular/core';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { RouterModule } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../../../core/service/auth/auth.service';
import { PermissionService } from '../../../core/service/permission/permission.service';
import { SweetAlertService } from '../../Services/sweet-alert/sweet-alert.service';
import { BackendMenuItem, SidebarItem } from './sidebar.config';

@Component({
  selector: 'app-sidebar',
  standalone: true,
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
  private permissionService = inject(PermissionService);
  private authService = inject(AuthService);
  private sweetAlertService = inject(SweetAlertService);

  @Output() closeSidebar = new EventEmitter<void>();

  readonly menu = computed(() => {
    const backendMenu = this.permissionService.menu();
    return this.transformMenu(backendMenu);
  });

  cerrarSesion(): void {
    this.sweetAlertService
      .showConfirm(
        '¿Estás seguro?',
        'Tu sesión actual se cerrará.',
        'Sí, salir',
        'Cancelar'
      )
      .then((result) => {
        if (result.isConfirmed) {
          this.sweetAlertService.showLoading(
            'Cerrando sesión',
            'Por favor, espera...'
          );
          this.authService
            .logout()
            .pipe(finalize(() => this.sweetAlertService.hideLoading()))
            .subscribe({
              error: (err) => {
                this.sweetAlertService.showNotification(
                  'Error',
                  err?.error?.message ??
                    'Ocurrió un error al cerrar la sesión.',
                  'error'
                );
              },
            });
        }
      });
  }

  private transformMenu(
    backendMenu: readonly BackendMenuItem[] | null
  ): SidebarItem[] {
    if (!backendMenu) return [];

    // Create copies before sorting to avoid mutating the original array from the signal
    const sortedMenu = [...backendMenu].sort((a, b) => a.id - b.id);

    return sortedMenu.map((menuItem) => ({
      label: menuItem.name,
      icon: menuItem.icon,
      children: (menuItem.forms ? [...menuItem.forms] : [])
        .sort((a, b) => a.id - b.id)
        .map((form) => ({
          label: form.name,
          icon: '',
          route: `/admin/${form.route}`,
        })),
    }));
  }
}
