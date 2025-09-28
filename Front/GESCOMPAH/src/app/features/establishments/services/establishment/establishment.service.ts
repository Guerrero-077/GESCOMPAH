import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment.development';
import {
  EstablishmentSelect,
  EstablishmentCreate,
  EstablishmentUpdate,
  EstablishmentCard
} from '../../models/establishment.models';

export interface GetAllOptions {
  /** Si true, trae solo activos; si no se envía, trae todos (Any). */
  activeOnly?: boolean;
  /** Límite máximo de registros a solicitar. */
  limit?: number;
}

@Injectable({ providedIn: 'root' })
export class EstablishmentService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiURL}/Establishments`;
  private readonly defaultLimit = environment.establishmentsDefaultLimit;

  // Consultas

  /**
   * Obtener establecimientos:
   * - sin opciones -> aplica límite por defecto (si existe) y trae todos (Any)
   * - { activeOnly: true } -> solo activos
   * - { limit: 25 } -> limita la respuesta
   */
  getAll(options?: GetAllOptions): Observable<EstablishmentSelect[]> {
    let params = new HttpParams();
    const limit = options?.limit ?? this.defaultLimit ?? undefined;
    if (options?.activeOnly) params = params.set('activeOnly', 'true');
    if (typeof limit === 'number' && limit > 0) params = params.set('limit', limit.toString());
    return this.http.get<EstablishmentSelect[]>(this.baseUrl, { params });
  }

  /** Listado liviano para cards/grids */
  getCards(options?: GetAllOptions): Observable<EstablishmentCard[]> {
    let params = new HttpParams();
    if (options?.activeOnly) params = params.set('activeOnly', 'true');
    return this.http.get<EstablishmentCard[]>(`${this.baseUrl}/cards`, { params });
  }

  getCardsAny(): Observable<EstablishmentCard[]> { return this.getCards(); }
  getCardsActive(): Observable<EstablishmentCard[]> { return this.getCards({ activeOnly: true }); }

  /** Conveniencias explícitas (opcionales) */
  getAllAny(limit?: number): Observable<EstablishmentSelect[]> {
    return this.getAll({ limit }); // Any
  }

  getAllActive(limit?: number): Observable<EstablishmentSelect[]> {
    return this.getAll({ activeOnly: true, limit });
  }

  /**
   * Obtener establecimientos por plaza.
   */
  getByPlaza(plazaId: number, options?: GetAllOptions): Observable<EstablishmentSelect[]> {
    let params = new HttpParams();
    const limit = options?.limit ?? this.defaultLimit ?? undefined;
    if (options?.activeOnly) params = params.set('activeOnly', 'true');
    if (typeof limit === 'number' && limit > 0) params = params.set('limit', limit.toString());
    return this.http.get<EstablishmentSelect[]>(`${this.baseUrl}/plaza/${plazaId}`, { params });
  }

  /** Detalle (puedes reutilizar activeOnly si tu backend lo soporta) */
  getById(id: number, activeOnly?: boolean): Observable<EstablishmentSelect> {
    let params = new HttpParams();
    if (activeOnly) params = params.set('activeOnly', 'true');
    return this.http.get<EstablishmentSelect>(`${this.baseUrl}/${id}`, { params });
  }

  // CRUD

  create(dto: EstablishmentCreate): Observable<EstablishmentSelect> {
    // Limpia campos locales (archivos/imágenes) si viajan desde el formulario
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
