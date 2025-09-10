import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment.development';
import {
  EstablishmentSelect,
  EstablishmentCreate,
  EstablishmentUpdate
} from '../../models/establishment.models';

export interface GetAllOptions {
  /** Si true, trae sólo activos; si no se envía, trae todos (Any). */
  activeOnly?: boolean;
}

@Injectable({ providedIn: 'root' })
export class EstablishmentService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiURL}/Establishments`;

  // ----------------------------- QUERIES -----------------------------

  /**
   * Obtener establecimientos:
   * - sin opciones -> todos (Any)
   * - { activeOnly: true } -> sólo activos
   */
  getAll(options?: GetAllOptions): Observable<EstablishmentSelect[]> {
    let params = new HttpParams();
    if (options?.activeOnly) params = params.set('activeOnly', 'true');
    return this.http.get<EstablishmentSelect[]>(this.baseUrl, { params });
  }

  /** Conveniencias explícitas (opcionales) */
  getAllAny(): Observable<EstablishmentSelect[]> {
    return this.getAll(); // Any
  }

  getAllActive(): Observable<EstablishmentSelect[]> {
    return this.getAll({ activeOnly: true });
  }

  /** Detalle (puedes reutilizar activeOnly si tu backend lo soporta) */
  getById(id: number, activeOnly?: boolean): Observable<EstablishmentSelect> {
    let params = new HttpParams();
    if (activeOnly) params = params.set('activeOnly', 'true');
    return this.http.get<EstablishmentSelect>(`${this.baseUrl}/${id}`, { params });
  }

  // ------------------------------ CRUD ------------------------------

  create(dto: EstablishmentCreate): Observable<EstablishmentSelect> {
    // Limpia campos locales (archivos/imagenes) si viajan desde el formulario
    const { files, images, imagesToDelete, ...body } = dto as any;
    return this.http.post<EstablishmentSelect>(this.baseUrl, body);
  }

  update(dto: EstablishmentUpdate): Observable<EstablishmentSelect> {
    if (!dto.id) throw new Error('ID del establecimiento es obligatorio');
    return this.http.put<EstablishmentSelect>(`${this.baseUrl}/${dto.id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  /**
   * Borrado lógico (si tu API lo implementa).
   * Ajusta el endpoint/método HTTP según tu backend.
   */
  deleteLogic(id: number): Observable<void> {
    // Común en backends: DELETE /{id}/logic  (si usas PATCH, cámbialo aquí)
    return this.http.delete<void>(`${this.baseUrl}/${id}/logic`);
  }

  /**
   * Cambio de estado activo/inactivo en backend.
   * El store hará optimismo y luego confirmará con la respuesta.
   */
  changeActiveStatus(id: number, active: boolean): Observable<EstablishmentSelect> {
    // Convención usada en tus otros controladores: PATCH /{id}/estado
    return this.http.patch<EstablishmentSelect>(`${this.baseUrl}/${id}/estado`, { active });
  }
}
