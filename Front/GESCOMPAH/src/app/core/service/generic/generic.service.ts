import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class GenericService<T> { // Pendiente por agregar los demas Dtos
  constructor(protected http: HttpClient) { }
  urlBase = environment.apiURL + '/';


  getAll(entidad: string): Observable<T[]> {
    return this.http.get<T[]>(`${this.urlBase}${entidad}`);
  }

  getById(entidad: string, id: number): Observable<T> {
    // Previous implementation concatenated the id without a slash,
    // producing URLs like `.../Entidad5` which are invalid.
    // Ensure the id is separated by a `/` to match REST conventions.
    return this.http.get<T>(`${this.urlBase}${entidad}/${id}`);
  }

  Add(entidad: string, dto: T): Observable<T> {
    return this.http.post<T>(`${this.urlBase}${entidad}`, dto);
  }

  Update(entidad: string, id: number, dto: T): Observable<T> {
    return this.http.put<T>(`${this.urlBase}${entidad}/${id}`, dto);
  }


  Delete(entidad: string, id: number): Observable<T> {
    return this.http.delete<T>(`${this.urlBase}${entidad}/${id}`);
  }

  DeleteLogical(entidad: string, id: number): Observable<T> {
    const body = { isDeleted: true }; // o el campo que uses
    return this.http.patch<T>(`${this.urlBase}${entidad}/${id}`, body);
  }
}
