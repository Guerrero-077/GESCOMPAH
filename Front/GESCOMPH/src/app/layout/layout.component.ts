import { Component, ViewChild, inject, OnInit } from '@angular/core';
import { MatDrawer } from '@angular/material/sidenav';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatDrawerContainer, MatDrawerContent } from '@angular/material/sidenav';
import { SidebarComponent } from '../shared/components/sidebar/sidebar.component';
import { HeaderComponent } from '../shared/components/header/header.component';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Observable } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';
import { PermissionsRealtimeService } from '../core/realtime/permissions-realtime.service';

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
export class LayoutComponent implements OnInit {
  @ViewChild(MatDrawer) drawer!: MatDrawer;

  private breakpointObserver = inject(BreakpointObserver);
  private readonly permissionsRt = inject(PermissionsRealtimeService);

  isHandset$: Observable<boolean> = this.breakpointObserver.observe(Breakpoints.Handset)
    .pipe(
      map(result => result.matches),
      shareReplay()
    );

  toggleSidebar = () => {
    this.drawer.toggle();
  };

  ngOnInit(): void {
    this.permissionsRt.connect();
  }
}
