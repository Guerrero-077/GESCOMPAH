import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';
import { Observable, of, tap } from 'rxjs';

import {
  ContractCreateModel,
  ContractSelectModel,
} from '../../models/contract.models';
import { ContractStore } from './contract.store';

@Injectable({
  providedIn: 'root',
})
export class ContractService {
  private readonly http = inject(HttpClient);
  private readonly store = inject(ContractStore);
  private readonly baseUrl = `${environment.apiURL}/contract`;

  readonly contracts = this.store.contracts;

  /**
   * Crea un contrato con posible creación de persona y/o usuario.
   */
  createContract(payload: ContractCreateModel): Observable<number> {
    return this.http.post<number>(`${this.baseUrl}`, payload).pipe(
      tap(() => {
        // After creating, reload all contracts to update the store
        this.getAllContracts({ force: true }).subscribe();
      })
    );
  }

  /**
   * Obtiene todos los contratos registrados.
   */
  getAllContracts(
    options: { force: boolean } = { force: false }
  ): Observable<ContractSelectModel[]> {
    const areContractsLoaded = this.store.contracts().length > 0;
    if (areContractsLoaded && !options.force) {
      return of(this.store.contracts());
    }

    return this.http.get<ContractSelectModel[]>(`${this.baseUrl}`).pipe(
      tap((contracts) => {
        this.store.setContracts(contracts);
      })
    );
  }

  /**
   * Obtiene un contrato por su ID.
   */
  getContractById(id: number): Observable<ContractSelectModel> {
    return this.http.get<ContractSelectModel>(`${this.baseUrl}/${id}`);
  }

  /**
   * Updates a contract
   */
  updateContract(id: number, payload: Partial<ContractCreateModel>): Observable<ContractSelectModel> {
    return this.http.put<ContractSelectModel>(`${this.baseUrl}/${id}`, payload).pipe(
      tap(updatedContract => {
        this.store.updateContract(updatedContract);
      })
    );
  }

  /**
   * Updates a contract active state
   */
  updateContractActive(id: number, active: boolean): Observable<ContractSelectModel> {
    // Optimista: actualiza el store primero y haz rollback si falla
    const prev = this.store.snapshot.find(c => c.id === id);
    if (prev) {
      this.store.updateContract({ ...prev, active });
    }

    return this.http
      .patch<ContractSelectModel>(`${this.baseUrl}/${id}/estado`, { active })
      .pipe(
        tap(updated => {
          // Asegura consistencia con lo que devuelve el backend
          this.store.updateContract(updated);
        })
      );
  }


  /**
   * Elimina un contrato por su ID.
   * @param id
   * @returns
   */
  deleteContract(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`).pipe(
      tap(() => {
        this.store.deleteContract(id);
      })
    );
  }

  downloadContractPdf(id: number): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/${id}/pdf`, {
      responseType: 'blob'
    });
  }


}

