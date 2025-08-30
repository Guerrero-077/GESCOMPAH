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

  // Señal pública para la tabla
  readonly rows = this.store.rows;

  // ------------------------------ CREATE ------------------------------
  createContract(payload: ContractCreateModel): Observable<number> {
    return this.http.post<number>(`${this.baseUrl}`, payload).pipe(
      tap(() => {
        // refresca la lista única (el backend decide por rol)
        this.getList({ force: true }).subscribe();

        // Impacta establecimientos en UI (si aplica)
        this.establishmentStore.patchActiveMany(payload.establishmentIds, false);
      })
    );
  }

  // ------------------------------- READ -------------------------------
  /**
   * ÚNICO método de listado para grilla.
   * Admin -> todos; Arrendador -> solo los suyos (lo decide el backend).
   */
  getList(options: { force?: boolean } = {}): Observable<ContractCard[]> {
    const force = !!options.force;
    if (this.store.rows().length > 0 && !force) return of(this.store.rows());
    return this.http.get<ContractCard[]>(`${this.baseUrl}/mine`, { withCredentials: true }).pipe(
      tap(list => this.store.setRows(list))
    );
  }

  /** Detalle puntual (ver/editar) */
  getContractById(id: number): Observable<ContractSelectModel> {
    return this.http.get<ContractSelectModel>(`${this.baseUrl}/${id}`);
  }

  // ------------------------------ UPDATE ------------------------------
  updateContract(id: number, payload: Partial<ContractCreateModel>): Observable<ContractSelectModel> {
    return this.http.put<ContractSelectModel>(`${this.baseUrl}/${id}`, payload).pipe(
      tap(updated => this.store.patchFromDetail(updated))
    );
  }

  /** Cambia estado activo/inactivo (optimista en grilla). */
  updateContractActive(id: number, active: boolean): Observable<ContractSelectModel> {
    this.store.updateRowActive(id, active); // optimista
    return this.http.patch<ContractSelectModel>(`${this.baseUrl}/${id}/estado`, { active }).pipe(
      tap(updated => this.store.updateRowActive(updated.id, updated.active)) // confirma
    );
  }

  // ------------------------------ DELETE ------------------------------
  deleteContract(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`).pipe(
      tap(() => this.store.deleteRow(id))
    );
  }

  // ------------------------------- PDF --------------------------------
  downloadContractPdf(id: number): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/${id}/pdf`, { responseType: 'blob' });
  }
}
