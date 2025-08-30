import { HttpClient, HttpEvent } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment.development';
import { ImageSelectDto } from '../../models/establishment.models';

@Injectable({ providedIn: 'root' })
export class ImageService {
  private readonly baseUrl = `${environment.apiURL}/images`;

  constructor(private http: HttpClient) { }

  /** Obtener imágenes de un establecimiento */
  getImagesByEstablishmentId(id: number): Observable<ImageSelectDto[]> {
    // Si tu backend expone GET /api/Images/{establishmentId}, puedes mapearlo aquí
    return this.http.get<ImageSelectDto[]>(`${this.baseUrl}/${id}`);
  }

  /** SUBIR imágenes (flujo 2da fase) → POST /api/Images/{establishmentId} */
  uploadImages(establishmentId: number, files: File[]): Observable<HttpEvent<unknown>> {
    const fd = new FormData();
    files.forEach(f => fd.append('files', f, f.name));
    return this.http.post(`${this.baseUrl}/${establishmentId}`, fd, {
      observe: 'events',
      reportProgress: true
    });
  }

  /** Eliminar varias imágenes por sus publicId (si tu backend lo soporta) */
  deleteImagesByPublicIds(publicIds: string[]): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/multiple`, { body: publicIds });
  }

  /** Eliminar lógicamente una imagen por su Id */
  deleteImageById(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  /** Eliminar lógicamente una imagen por su publicId */
  logicalDeleteImage(publicId: string): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/logical-delete`, null, { params: { publicId } });
  }
}
