import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment.development';
import { EstablishmentSelect, EstablishmentCreate, EstablishmentUpdate } from '../../models/establishment.models';

@Injectable({ providedIn: 'root' })
export class EstablishmentService {
  /** Base URL del API (apiURL + /Establishments) */
  private readonly urlBase = `${environment.apiURL}/Establishments`;

  constructor(private http: HttpClient) {}

  /** --------------------------------------------------  CRUD  ----------------------------------------------------- */

  /** Obtener todos los establecimientos  */
  getAll(): Observable<EstablishmentSelect[]> {
    return this.http.get<EstablishmentSelect[]>(this.urlBase);
  }

  /** Obtener un establecimiento por ID */
  getById(id: number): Observable<EstablishmentSelect> {
    return this.http.get<EstablishmentSelect>(`${this.urlBase}/${id}`);
  }

  /** Eliminar un establecimiento por ID */
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.urlBase}/${id}`);
  }

  /** ------------------------  CREATE (JSON)  ------------------------- */
  create(dto: EstablishmentCreate): Observable<EstablishmentSelect> {
    // Asegúrate de NO enviar File[] en el cuerpo
    const { files, images, imagesToDelete, ...body } = dto as any;
    return this.http.post<EstablishmentSelect>(this.urlBase, body);
  }

  /** ------------------------  UPDATE (JSON)  ------------------------- */
update(dto: EstablishmentUpdate): Observable<EstablishmentSelect> {
  if (!dto.id) throw new Error('ID del establecimiento es obligatorio');
  return this.http.put<EstablishmentSelect>(`${this.urlBase}/${dto.id}`, dto);
}

}
