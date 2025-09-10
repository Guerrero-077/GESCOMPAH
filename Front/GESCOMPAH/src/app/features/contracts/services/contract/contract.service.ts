import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  ContractCreateModel,
  ContractSelectModel,
  ContractCard,
  MonthlyObligation,
} from '../../models/contract.models';
import { environment } from '../../../../../environments/environment.development';

@Injectable({ providedIn: 'root' })
export class ContractService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiURL}/contract`;

  /** Listado para la grilla (según tu API, aquí uso /mine). */
  getAll(_options: { force?: boolean } = {}): Observable<ContractCard[]> {
    return this.http.get<ContractCard[]>(`${this.baseUrl}/mine`, { withCredentials: true });
  }

  /** Detalle puntual (ver/editar). */
  getById(id: number): Observable<ContractSelectModel> {
    return this.http.get<ContractSelectModel>(`${this.baseUrl}/${id}`);
  }

  /** Crear contrato. Recomendado: el backend devuelve el `id` creado. */
  create(payload: ContractCreateModel): Observable<number> {
    return this.http.post<number>(`${this.baseUrl}`, payload);
  }

  /** Actualizar contrato (PUT). Devuelve el detalle actualizado. */
  update(id: number, payload: Partial<ContractCreateModel>): Observable<ContractSelectModel> {
    return this.http.put<ContractSelectModel>(`${this.baseUrl}/${id}`, payload);
  }

  /** Cambiar estado (PATCH /{id}/estado). Devuelve la entidad (o al menos `active`). */
  changeActiveStatus(id: number, active: boolean): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/${id}/estado`, { active });
  }

  /** Borrado físico. */
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  /**
   * Borrado lógico (si tu API lo soporta). Si no existe endpoint, puedes
   * redirigirlo a `delete(id)` o crear el real cuando esté listo.
   */
  deleteLogic(id: number): Observable<void> {
    // Fallback al delete físico; cámbialo por tu endpoint de soft-delete cuando exista.
    return this.delete(id);
  }

  /** Descargar PDF. */
  downloadContractPdf(id: number): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/${id}/pdf`, { responseType: 'blob' });
  }

  /**
   * Listar obligaciones mensuales asociadas a un contrato.
   * Ajusta el endpoint si tu backend usa otra ruta.
   */
  getMonthlyObligations(contractId: number): Observable<MonthlyObligation[]> {
    return this.http.get<MonthlyObligation[]>(`${this.baseUrl}/${contractId}/obligations`);
  }
}
