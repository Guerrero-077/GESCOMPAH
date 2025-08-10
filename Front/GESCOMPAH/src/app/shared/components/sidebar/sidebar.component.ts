import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatExpansionModule } from '@angular/material/expansion';
import { SidebarItem, BackendMenuItem, BackendSubMenuItem } from './sidebar.config';
import { PermissionService } from '../../../core/service/permission/permission.service'; // Import PermissionService


@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule, MatListModule, MatIconModule, MatExpansionModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent implements OnInit {
  expandedIndex = -1;
  menu: SidebarItem[] = [];

  constructor(private permissionService: PermissionService) { } // Inject PermissionService

  ngOnInit(): void {
    this.permissionService.userProfile$.subscribe(userProfile => {
      if (userProfile && userProfile.menu) {
        this.menu = this.transformMenu(userProfile.menu);
      } else {
        this.menu = [];
      }
    });
  }

  cerrarSesion() {
    console.log('Cerrar sesión');
  }

  private transformMenu(backendMenu: BackendMenuItem[]): SidebarItem[] {
    const sidebarItems: SidebarItem[] = [];
    backendMenu.forEach(menuItem => {
      const children: SidebarItem[] = [];
      menuItem.forms.forEach(form => {
        children.push({
          label: form.name,
          icon: '', // Sub-items might not have icons, or you can map them
          route: `/Admin/${form.route}` // Prefix with /Admin/
        });
      });

      sidebarItems.push({
        label: menuItem.name,
        icon: menuItem.icon,
        children: children
      });
    });
    return sidebarItems;
  }
}
