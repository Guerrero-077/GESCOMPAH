import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ImageSelectDto } from '../../Models/Establishment.models';

@Injectable({ providedIn: 'root' })
export class ImageService {
  private readonly baseUrl = `${environment.apiURL}/images`;

  constructor(private http: HttpClient) { }

  /** Obtener imágenes de un establecimiento */
  getImagesByEstablishmentId(id: number): Observable<ImageSelectDto[]> {
    return this.http.get<ImageSelectDto[]>(`${this.baseUrl}/establishment/${id}`);
  }

  /** Eliminar varias imágenes a la vez mediante sus publicId */
  deleteImagesByPublicIds(publicIds: string[]): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/by-publicIds`, { body: publicIds });
  }

  /** Eliminar lógicamente una imagen por su publicId */
  logicalDeleteImage(publicId: string): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/logical-delete`, null, { params: { publicId } });
  }


}
