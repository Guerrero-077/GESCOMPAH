import { inject, Injectable } from '@angular/core';
import { FormModuleCreateDto, FormModuleSelectDto, FormModuleUpdateDto } from '../../models/form-module..models';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { RolUserCreateDto } from '../../models/rol-user.models';

@Injectable({
  providedIn: 'root'
})
export class FormModuleService {

  // Additional methods specific to RolUser can be added here
  private readonly http = inject(HttpClient);

  private readonly baseUrl = `${environment.apiURL}/FormModule`;

  getAll(): Observable<FormModuleSelectDto[]> {
    return this.http.get<FormModuleSelectDto[]>(this.baseUrl);
  }

  getById(id: number): Observable<FormModuleSelectDto> {
    return this.http.get<FormModuleSelectDto>(`${this.baseUrl}/${id}`);
  }

  create(payload: FormModuleCreateDto): Observable<FormModuleSelectDto> {
    return this.http.post<FormModuleSelectDto>(this.baseUrl, payload);
  }

  update(id: number, payload: FormModuleUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, payload);
  }

  deleteLogical(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
