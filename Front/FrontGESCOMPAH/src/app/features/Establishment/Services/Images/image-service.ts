import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ImageSelectDto } from '../../Models/Establishment.models';
import { environment } from '../../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ImageService {
  /**
   * Obtener todas las imágenes asociadas a un establecimiento
   * @param establishmentId ID del establecimiento
  */


  UrlBase = environment.apiURL + '/image';
  constructor(private http: HttpClient) { }

  getImagesByEstablishmentId(establishmentId: number): Observable<ImageSelectDto[]> {
    return this.http.get<ImageSelectDto[]>(`${this.UrlBase}/establishment/${establishmentId}`);
  }

  /**
   * Subir imágenes a un establecimiento (máximo 5 por establecimiento)
   * @param establishmentId ID del establecimiento
   * @param files Lista de archivos de imagen
   */
  uploadImages(establishmentId: number, files: File[]): Observable<ImageSelectDto[]> {
    const formData = new FormData();
    files.forEach(file => {
      formData.append('files', file, file.name);
    });

    return this.http.post<ImageSelectDto[]>(
      `${this.UrlBase}/establishment/${establishmentId}`,
      formData
    );
  }

  /**
   * Eliminar una imagen por su ID
   * @param imageId ID de la imagen a eliminar
   */
  deleteImageById(imageId: number): Observable<void> {
    return this.http.delete<void>(`${this.UrlBase}/${imageId}`);
  }

  /**
   * Eliminar múltiples imágenes usando una lista de publicIds
   * @param publicIds Lista de publicIds a eliminar
   */
  deleteImagesByPublicIds(publicIds: string[]): Observable<void> {
    return this.http.delete<void>(`${this.UrlBase}/by-publicIds`, {
      body: publicIds
    });
  }
}
