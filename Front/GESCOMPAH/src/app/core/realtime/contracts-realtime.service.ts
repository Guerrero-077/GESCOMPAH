// src/app/core/service/realtime/contracts-realtime.service.ts
import { Injectable, inject, NgZone, ApplicationRef } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment.development';
import { ContractStore } from '../../features/contracts/services/contract/contract.store';

type ExpiredPayload = {
  deactivatedIds?: number[];
  counts?: { deactivatedContracts: number; reactivatedEstablishments: number };
  at?: string;
};

@Injectable({ providedIn: 'root' })
export class ContractsRealtimeService {
  private readonly store = inject(ContractStore);
  private readonly zone = inject(NgZone);
  private readonly appRef = inject(ApplicationRef);
  private hub?: signalR.HubConnection;
  private handlersBound = false;
  private refreshing = false;

  connect(): void {
    if (this.hub) return;

    const hubUrl = `${environment.apiURL.replace(/\/$/, '')}/hubs/contracts`;

    this.hub = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, { withCredentials: true })
      .withAutomaticReconnect()
      .build();

    if (!this.handlersBound) {
      this.bindHandlers();
      this.handlersBound = true;
    }

    this.hub.start()
      .then(() => console.debug('[SignalR] Conectado a', hubUrl))
      .catch(err => console.error('[SignalR] Error de conexión', err));
  }

  disconnect(): void {
    this.hub?.stop().catch(() => void 0);
    this.hub = undefined;
    this.handlersBound = false;
  }

  // --- privados ---
  private bindHandlers(): void {
    // Evento genérico (si lo usas en otras partes)
    this.hub!.on('contracts:mutated', (payload: { type: string; id: number; active?: boolean }) => {
      this.zone.run(() => {
        switch (payload.type) {
          case 'statusChanged':
            if (payload.id && payload.active !== undefined) {
              this.store.patchOne(payload.id, { active: payload.active });
              this.appRef.tick();
            } else {
              this.refreshAll();
            }
            break;
          case 'deleted':
            if (payload.id) {
              this.store.remove(payload.id);
              this.appRef.tick();
            } else {
              this.refreshAll();
            }
            break;
          case 'created':
            this.refreshAll();
            break;
        }
      });
    });

    // ✅ Worker: desactivaciones por expiración → parchear solo esas filas
    this.hub!.on('contracts:expired', (payload: ExpiredPayload) => {
      this.zone.run(() => {
        const ids = payload?.deactivatedIds ?? [];
        if (ids.length === 0) return; // nada que hacer

        this.store.patchActiveMany(ids, false);
        this.appRef.tick();
      });
    });

    this.hub!.onreconnected((_id) => {
      // opcional: re-sync completo tras reconexión
      this.zone.run(() => this.refreshAll());
    });

    this.hub!.onclose(err => {
      if (err) console.warn('[SignalR] Closed with error', err);
    });
  }

  private async refreshAll() {
    if (this.refreshing) return;
    this.refreshing = true;
    try {
      await this.store.loadAll({ force: true });
      this.appRef.tick();
    } finally {
      this.refreshing = false;
    }
  }
}
