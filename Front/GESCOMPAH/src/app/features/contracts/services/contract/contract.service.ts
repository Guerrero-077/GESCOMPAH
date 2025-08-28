import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';
import { Observable } from 'rxjs';

import { ContractCreateModel, ContractSelectModel } from '../../models/contract.models';

@Injectable({
  providedIn: 'root'
})
export class ContractService {
  private baseUrl = `${environment.apiURL}/contract`;

  constructor(private http: HttpClient) {}

  /**
   * Crea un contrato con posible creación de persona y/o usuario.
   */
  createContract(payload: ContractCreateModel): Observable<number> {
    return this.http.post<number>(`${this.baseUrl}`, payload);
  }

  /**
   * Obtiene todos los contratos registrados.
   */
  getAllContracts(): Observable<ContractSelectModel[]> {
    return this.http.get<ContractSelectModel[]>(`${this.baseUrl}`);
  }

  /**
   * Obtiene un contrato por su ID.
   */
  getContractById(id: number): Observable<ContractSelectModel> {
    return this.http.get<ContractSelectModel>(`${this.baseUrl}/${id}`);
  }
}
