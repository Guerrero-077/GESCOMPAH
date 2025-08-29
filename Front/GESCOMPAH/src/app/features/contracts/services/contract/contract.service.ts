import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, tap } from 'rxjs';

import {
  ContractCreateModel,
  ContractSelectModel,
  ContractCard
} from '../../models/contract.models';
import { ContractStore } from './contract.store';
import { EstablishmentStore } from '../../../establishments/services/establishment/establishment.store';
import { environment } from '../../../../../environments/environment.development';

@Injectable({ providedIn: 'root' })
export class ContractService {
  private readonly http  = inject(HttpClient);
  private readonly store = inject(ContractStore);
  private readonly establishmentStore = inject(EstablishmentStore);
  private readonly baseUrl = `${environment.apiURL}/contract`;

  // Señales públicas
  readonly contracts = this.store.contracts; // lista completa
  readonly cards     = this.store.cards;     // lista “mine”

  // ------------------------------ CREATE ------------------------------
  createContract(payload: ContractCreateModel): Observable<number> {
    return this.http.post<number>(`${this.baseUrl}`, payload).pipe(
      tap(() => {
        // refrescar ambas listas si tienen pantallas abiertas
        this.getAllContracts({ force: true }).subscribe();
        this.getMineCards({ force: true }).subscribe();
        // Marcar locales como inactivos en el store del módulo de establecimientos
        this.establishmentStore.patchActiveMany(payload.establishmentIds, false);
      })
    );
  }

  // ------------------------------- READ -------------------------------
  /** Admin: obtiene la lista COMPLETA. */
  getAllContracts(options: { force?: boolean } = {}): Observable<ContractSelectModel[]> {
    const force = !!options.force;
    if (this.store.contracts().length > 0 && !force) return of(this.store.contracts());
    return this.http.get<ContractSelectModel[]>(`${this.baseUrl}`).pipe(
      tap(list => this.store.setContracts(list))
    );
  }

  /** Admin/Arrendador: obtiene /contract/mine (Admin → todos, Arrendador → los suyos). */
  getMineCards(options: { force?: boolean } = {}): Observable<ContractCard[]> {
    const force = !!options.force;
    if (this.store.cards().length > 0 && !force) return of(this.store.cards());
    return this.http.get<ContractCard[]>(`${this.baseUrl}/mine`, { withCredentials: true }).pipe(
      tap(list => this.store.setCards(list))
    );
  }

  getContractById(id: number): Observable<ContractSelectModel> {
    return this.http.get<ContractSelectModel>(`${this.baseUrl}/${id}`);
  }

  // ------------------------------ UPDATE ------------------------------
  updateContract(id: number, payload: Partial<ContractCreateModel>): Observable<ContractSelectModel> {
    return this.http.put<ContractSelectModel>(`${this.baseUrl}/${id}`, payload).pipe(
      tap(updated => {
        this.store.updateContract(updated);
        this.store.patchCardIfDatesMatch(updated);
      })
    );
  }

  updateContractActive(id: number, active: boolean): Observable<ContractSelectModel> {
    // Optimista
    const prev = this.store.snapshot.find(c => c.id === id);
    if (prev) this.store.updateContract({ ...prev, active });

    return this.http.patch<ContractSelectModel>(`${this.baseUrl}/${id}/estado`, { active }).pipe(
      tap(updated => {
        this.store.updateContract(updated);
        this.store.patchCardActive(updated.id, updated.active);
      })
    );
  }

  // ------------------------------ DELETE ------------------------------
  deleteContract(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`).pipe(
      tap(() => {
        this.store.deleteContract(id);
        this.store.deleteCard(id);
      })
    );
  }

  // ------------------------------- PDF --------------------------------
  downloadContractPdf(id: number): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/${id}/pdf`, { responseType: 'blob' });
  }
}
