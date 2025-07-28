import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment.development';
import {
  EstablishmentCreate,
  EstablishmentSelect,
  EstablishmentUpdate
} from '../../Models/Establishment.models';

@Injectable({ providedIn: 'root' })
export class LocalesService {
  private urlBase = `${environment.apiURL}/Establishments`;

  constructor(private http: HttpClient) { }

  /** Obtener todos los establecimientos (admin o público) */
  getAll(): Observable<EstablishmentSelect[]> {
    return this.http.get<EstablishmentSelect[]>(this.urlBase);
  }

  /** Obtener un establecimiento por ID */
  getById(id: number): Observable<EstablishmentSelect> {
    return this.http.get<EstablishmentSelect>(`${this.urlBase}/${id}`);
  }

  /** Crear un nuevo establecimiento con imágenes */
  create(establishment: EstablishmentCreate): Observable<EstablishmentSelect> {
    const formData = this.buildFormData(establishment);
    return this.http.post<EstablishmentSelect>(this.urlBase, formData);
  }

  /** Actualizar un establecimiento, incluyendo imágenes nuevas y existentes */
  update(establishment: EstablishmentUpdate): Observable<EstablishmentSelect> {
    if (!establishment.id) throw new Error('Establishment ID is required for update');

    const formData = this.buildFormData(establishment);
    return this.http.put<EstablishmentSelect>(
      `${this.urlBase}/${establishment.id}`,
      formData
    );
  }

  /** Eliminar un establecimiento por ID */
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.urlBase}/${id}`);
  }

  /**
   * Armar el objeto FormData compatible con multipart/form-data para el backend.
   * Aplica tanto para creación como para edición.
   */
  private buildFormData(est: EstablishmentCreate | EstablishmentUpdate): FormData {
    const form = new FormData();

    // Datos básicos
    form.append('name', est.name);
    form.append('description', est.description);
    form.append('areaM2', est.areaM2.toString());
    form.append('rentValueBase', est.rentValueBase.toString());
    form.append('plazaId', est.plazaId.toString());
    if ('address' in est && est.address) {
      form.append('address', est.address);
    }

    // Si es actualización
    if ('id' in est) {
      form.append('id', est.id.toString());

      // Imágenes existentes que se mantienen
      if (est.existingImages?.length) {
        est.existingImages.forEach((img, i) => {
          form.append(`existingImages[${i}].id`, img.id.toString());
          form.append(`existingImages[${i}].fileName`, img.fileName);
          form.append(`existingImages[${i}].filePath`, img.filePath);
          form.append(`existingImages[${i}].publicId`, img.publicId);
        });
      }
    }

    // Nuevas imágenes a subir
    if (est.files?.length) {
      est.files.forEach(file => {
        form.append('files', file, file.name);
      });
    }

    return form;
  }
}
