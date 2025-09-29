import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { AuthEventsService } from './core/security/services/auth/auth-events.service';
import { SweetAlertService } from './shared/Services/sweet-alert/sweet-alert.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CommonModule],
  standalone: true,
  providers: [],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  router = inject(Router);
  private authEvents: AuthEventsService = inject(AuthEventsService);
  private toast: SweetAlertService = inject(SweetAlertService);
  protected title = 'FrontGESCOMPAH';

  constructor() {
    // Listener global: ante expiración o logout forzado, mostrar mensaje y llevar a login
    this.authEvents.onEvents().subscribe(async (ev) => {
      if (ev.type === 'SESSION_EXPIRED') {
        await this.toast.error('Tu sesión ha expirado. Vuelve a iniciar sesión.', 'Sesión expirada');
        this.router.navigate(['/auth/login']);
      }
      if (ev.type === 'LOGOUT') {
        await this.toast.success('Has cerrado sesión correctamente.', 'Sesión cerrada');
        this.router.navigate(['/auth/login']);
      }
    });
  }
}
