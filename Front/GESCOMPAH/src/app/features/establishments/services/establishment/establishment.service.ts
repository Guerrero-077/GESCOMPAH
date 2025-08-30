import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment.development';
import { EstablishmentSelect, EstablishmentCreate, EstablishmentUpdate } from '../../models/establishment.models';

export interface GetAllOptions {
  /** Si true, trae solo activos; si no lo envías, trae todos (Any). */
  activeOnly?: boolean;
}

@Injectable({ providedIn: 'root' })
export class EstablishmentService {
  private readonly urlBase = `${environment.apiURL}/Establishments`;

  constructor(private http: HttpClient) {}

  /** -------------------------------  QUERIES  -------------------------------- */

  /** Obtener establecimientos (por defecto TODOS; usa activeOnly=true para SOLO activos) */
  getAll(options?: GetAllOptions): Observable<EstablishmentSelect[]> {
    let params = new HttpParams();
    if (options?.activeOnly === true) params = params.set('activeOnly', 'true');
    return this.http.get<EstablishmentSelect[]>(this.urlBase, { params });
  }

  /** Conveniencias explícitas (opcional) */
  getAllAny(): Observable<EstablishmentSelect[]> {
    return this.getAll(); // sin params → Any
  }

  getAllActive(): Observable<EstablishmentSelect[]> {
    return this.getAll({ activeOnly: true });
  }

  /** Obtener un establecimiento por ID (añade activeOnly=true si requieres que sea activo) */
  getById(id: number, activeOnly?: boolean): Observable<EstablishmentSelect> {
    let params = new HttpParams();
    if (activeOnly === true) params = params.set('activeOnly', 'true');
    return this.http.get<EstablishmentSelect>(`${this.urlBase}/${id}`, { params });
  }

  /** ---------------------------------  CRUD  --------------------------------- */

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.urlBase}/${id}`);
  }

  create(dto: EstablishmentCreate): Observable<EstablishmentSelect> {
    const { files, images, imagesToDelete, ...body } = dto as any; // limpiar payload
    return this.http.post<EstablishmentSelect>(this.urlBase, body);
  }

  update(dto: EstablishmentUpdate): Observable<EstablishmentSelect> {
    if (!dto.id) throw new Error('ID del establecimiento es obligatorio');
    return this.http.put<EstablishmentSelect>(`${this.urlBase}/${dto.id}`, dto);
  }
}
