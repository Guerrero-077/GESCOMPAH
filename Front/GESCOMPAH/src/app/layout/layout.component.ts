import { Component, ViewChild } from '@angular/core';
import { MatDrawer } from '@angular/material/sidenav';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatDrawerContainer, MatDrawerContent } from '@angular/material/sidenav';
import { SidebarComponent } from '../shared/components/sidebar/sidebar.component';
import { HeaderComponent } from '../shared/components/header/header.component';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [
    CommonModule,
    SidebarComponent,
    HeaderComponent,
    RouterOutlet,
    MatIconModule,
    MatDrawerContainer,
    MatDrawer,
    MatDrawerContent,
  ],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.css',
})
export class LayoutComponent {
  @ViewChild(MatDrawer) drawer!: MatDrawer;

  toggleSidebar = () => {
    this.drawer.toggle();
  };
}
