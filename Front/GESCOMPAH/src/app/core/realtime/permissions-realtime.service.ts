import { ApplicationRef, Injectable, NgZone, inject } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import { AuthService } from '../security/services/auth/auth.service';
import { UserStore } from '../security/services/permission/User.Store';

type PermissionsUpdatedPayload = { userIds?: number[] };

@Injectable({ providedIn: 'root' })
export class PermissionsRealtimeService {
  private readonly auth = inject(AuthService);
  private readonly userStore = inject(UserStore);
  private readonly zone = inject(NgZone);
  private readonly appRef = inject(ApplicationRef);

  private hub?: signalR.HubConnection;
  private handlersBound = false;
  private refreshing = false;

  connect(): void {
    if (this.hub) return;

    const hubUrl = `${environment.apiURL.replace(/\/$/, '')}/hubs/security`;
    this.hub = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, { withCredentials: true })
      .withAutomaticReconnect()
      .build();

    if (!this.handlersBound) {
      this.bindHandlers();
      this.handlersBound = true;
    }

    this.hub.start()
      .then(() => console.debug('[SignalR] SecurityHub conectado a', hubUrl))
      .catch(err => console.error('[SignalR] Error de conexión SecurityHub', err));
  }

  disconnect(): void {
    this.hub?.stop().catch(() => void 0);
    this.hub = undefined;
    this.handlersBound = false;
  }

  private bindHandlers(): void {
    this.hub!.on('permissions:updated', (payload: PermissionsUpdatedPayload) => {
      this.zone.run(() => this.handlePermissionsUpdated(payload));
    });

    this.hub!.onreconnected((_id) => {
      this.zone.run(() => this.refreshMe());
    });

    this.hub!.onclose(err => {
      if (err) console.warn('[SignalR] SecurityHub closed', err);
    });
  }

  private handlePermissionsUpdated(payload: PermissionsUpdatedPayload) {
    const ids = payload?.userIds ?? [];
    const me = this.userStore.snapshot?.id;
    if (ids.length > 0 && (me == null || !ids.includes(me))) {
      return; // no aplica a este usuario
    }
    this.refreshMe();
  }

  private refreshMe() {
    if (this.refreshing) return;
    this.refreshing = true;
    this.auth.GetMe().subscribe({
      next: () => this.appRef.tick(),
      error: () => { this.refreshing = false; },
      complete: () => { this.refreshing = false; }
    });
  }
}
