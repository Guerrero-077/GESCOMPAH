import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { RolFormPermissionSelectDto, RolFormPermissionCreateDto, RolFormPermissionUpdateDto } from '../../models/rol-form-permission.models';

@Injectable({
  providedIn: 'root'
})
export class RolFormPermissionService {
  // Additional methods specific to RolUser can be added here
  private readonly http = inject(HttpClient);

  private readonly baseUrl = `${environment.apiURL}/rolFormPermission`;

  getAll(): Observable<RolFormPermissionSelectDto[]> {
    return this.http.get<RolFormPermissionSelectDto[]>(this.baseUrl);
  }

  getById(id: number): Observable<RolFormPermissionSelectDto> {
    return this.http.get<RolFormPermissionSelectDto>(`${this.baseUrl}/${id}`);
  }

  create(payload: RolFormPermissionCreateDto): Observable<RolFormPermissionSelectDto> {
    return this.http.post<RolFormPermissionSelectDto>(this.baseUrl, payload);
  }

  update(id: number, payload: RolFormPermissionUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, payload);
  }

  deleteLogical(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
