import { Component, computed, inject, Input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { PageHeaderService } from '../../Services/PageHeader/page-header.service';
import { UserStore } from '../../../core/security/services/permission/User.Store';
import { AuthService } from '../../../core/security/services/auth/auth.service';
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [MatIconModule, MatMenuModule, MatButtonModule],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css'],
})
export class HeaderComponent {
  @Input() toggleDrawer!: () => void;
  @Input() title!: string;
  @Input() description!: string;

  private header = inject(PageHeaderService);
  private userStore = inject(UserStore);
  private authService = inject(AuthService);

  // Si el servicio tiene valor → lo usa; si no, cae al @Input del layout (route data)
  displayTitle = computed(() => this.header.title() || this.title || '');
  displayDescription = computed(
    () => this.header.description() || this.description || ''
  );

  username = computed(() => this.userStore.user()?.fullName);

  logout() {
    this.authService.logout().subscribe();
  }
}
