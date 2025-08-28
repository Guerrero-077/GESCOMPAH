import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment.development';
import { EstablishmentSelect, EstablishmentCreate, EstablishmentUpdate } from '../../models/establishment.models';

@Injectable({
  providedIn: 'root'
})
export class EstablishmentService {
  /** Base URL del API (apiURL + /Establishments) */
  private readonly urlBase = `${environment.apiURL}/Establishments`;

  constructor(private http: HttpClient) { }

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

  /** ------------------------  CREATE  ------------------------- */
  create(est: EstablishmentCreate): Observable<EstablishmentSelect> {
    const fd = this.buildFormData(est);
    return this.http.post<EstablishmentSelect>(this.urlBase, fd);
  }

  /** ------------------------  UPDATE  ------------------------- */
  update(est: EstablishmentUpdate): Observable<EstablishmentSelect> {
    if (!est.id) throw new Error('ID del establecimiento es obligatorio');

    const fd = this.buildFormData(est);
    return this.http.put<EstablishmentSelect>(`${this.urlBase}/${est.id}`, fd);
  }

  /**
   * *FormData* de multipart/form‑data
   *
   * Se encarga de montar el objeto que el *ASP.NET Core* espera.
   * <br><br>
   * <b>Campos que envía</b>
   * | Campo | Tipo | Comentario |
   * |-------|------|------------|
   * | name   | string | mandatory |
   * | description | string | mandatory |
   * | areaM2 | number |
   * | rentValueBase | number |
   * | plazaId | number |
   * | address | string (opcional) |
   *
   * • Cuando se trata de *CREATE* se envía la colección `files` con los nuevos archivos.<br>
   * • Cuando se trata de *UPDATE* se envían `images` (nuevos archivos) y `imagesToDelete` (publicId de las que se borran).
   */
  private buildFormData(dto: EstablishmentCreate | EstablishmentUpdate): FormData {
    const data = new FormData();

    // -------- Campos básicos --------
    if (dto.name) data.append('name', dto.name);
    if (dto.description) data.append('description', dto.description);

    if (dto.areaM2 !== undefined) data.append('areaM2', dto.areaM2.toString());
    if (dto.uvtQty !== undefined) data.append('uvtQty', dto.uvtQty.toString());          // <-- FALTABA
    if (dto.rentValueBase !== undefined) data.append('rentValueBase', dto.rentValueBase.toString());

    if (dto.plazaId !== undefined) data.append('plazaId', dto.plazaId.toString());
    if (dto.address !== null && dto.address !== undefined) data.append('address', dto.address);

    // -------- Update opcional --------
    if ('id' in dto) data.append('id', dto.id.toString());

    // -------- Imágenes --------
    if ('files' in dto && dto.files?.length) {
      dto.files.forEach(f => data.append('files', f, f.name));
    } else if ('images' in dto && dto.images?.length) {
      dto.images.forEach(f => data.append('images', f, f.name));
    }

    if ('imagesToDelete' in dto && dto.imagesToDelete?.length) {
      // Si tu backend NO espera JSON, envíalas una por una:
      // dto.imagesToDelete.forEach(x => data.append('imagesToDelete', x));
      data.append('imagesToDelete', JSON.stringify(dto.imagesToDelete));
    }

    return data;
  }

}
