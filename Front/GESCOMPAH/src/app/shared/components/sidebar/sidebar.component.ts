import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/service/auth/auth.service';
import { PermissionService } from '../../../core/service/permission/permission.service'; // Import PermissionService
import { BackendMenuItem, SidebarItem } from './sidebar.config';


@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule, MatListModule, MatIconModule, MatExpansionModule],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {
  menu: SidebarItem[] = [];

  constructor(private permissionService: PermissionService, private authService: AuthService) { }

  ngOnInit(): void {
    this.permissionService.userProfile$.subscribe(userProfile => {
      console.log(userProfile);
      if (userProfile && userProfile.menu) {
        this.menu = this.transformMenu(userProfile.menu);
      } else {
        this.menu = [];
      }
    });
  }

  cerrarSesion() {
    this.authService.logout().subscribe();
  }

  private transformMenu(backendMenu: BackendMenuItem[]): SidebarItem[] {
    // Sort the main menu items by their 'id' property
    backendMenu.sort((a, b) => a.id - b.id);

    const sidebarItems: SidebarItem[] = [];
    backendMenu.forEach(menuItem => {
      const children: SidebarItem[] = [];
      // Sort the forms (sub-menu items) within each main menu item by their 'id' property
      menuItem.forms.sort((a, b) => a.id - b.id);

      menuItem.forms.forEach(form => {
        children.push({
          label: form.name,
          icon: '',
          route: `/admin/${form.route}`
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
