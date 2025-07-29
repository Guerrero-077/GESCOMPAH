import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SIDEBAR_BY_ROLE, SidebarItem } from './sidebar.config';
import { AuthService } from '../../../Core/Auth/auth-service';
// import { AuthService } from '../services/auth.service';
// import { MENU_BY_ROLE, MenuItem } from '../config/menu.config';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent {
  // menu: SidebarItem[] = [];
  menu: SidebarItem[] = SIDEBAR_BY_ROLE['admin'];

  // constructor(private authService: AuthService) {
  //   this.authService.GetMe().subscribe((user) => {
  //     const rol = user.roles[0]; 
  //     this.menu = SIDEBAR_BY_ROLE[rol]; 
  //   });
  // } 

  cerrarSesion() {
    console.log('Cerrar sesión');
  }
}
