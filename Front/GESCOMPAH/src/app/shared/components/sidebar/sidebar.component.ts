import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output, computed, inject } from '@angular/core';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/service/auth/auth.service';
import { PermissionService } from '../../../core/service/permission/permission.service';
import { BackendMenuItem, SidebarItem } from './sidebar.config';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule, MatListModule, MatIconModule, MatExpansionModule],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent {
  private permissionService = inject(PermissionService);
  private authService = inject(AuthService);

  @Output() closeSidebar = new EventEmitter<void>();

  readonly menu = computed(() => {
    const backendMenu = this.permissionService.menu();
    return this.transformMenu(backendMenu);
  });

  cerrarSesion(): void {
    this.authService.logout().subscribe();
  }

  private transformMenu(backendMenu: readonly BackendMenuItem[] | null): SidebarItem[] {
    if (!backendMenu) return [];

    // Create copies before sorting to avoid mutating the original array from the signal
    const sortedMenu = [...backendMenu].sort((a, b) => a.id - b.id);

    return sortedMenu.map(menuItem => ({
      label: menuItem.name,
      icon: menuItem.icon,
      children: (menuItem.forms ? [...menuItem.forms] : [])
        .sort((a, b) => a.id - b.id)
        .map(form => ({
          label: form.name,
          icon: '',
          route: `/admin/${form.route}`
        }))
    }));
  }
}
