import { inject, Injectable } from '@angular/core';
import { GenericService } from '../../../../core/service/generic/generic.service';
import { RolUserCreateDto, RolUserListDto, RolUserUpdatePayload } from '../../models/rol-user.models';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RolUserService {

  // Additional methods specific to RolUser can be added here
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiURL}/rolUser`; // usa lowercase para la ruta

  getAll(): Observable<RolUserListDto[]> {
    return this.http.get<RolUserListDto[]>(this.baseUrl);
  }


  getById(id: number): Observable<RolUserListDto> {
    return this.http.get<RolUserListDto>(`${this.baseUrl}/${id}`);
  }

  create(payload: RolUserCreateDto): Observable<RolUserListDto> {
    return this.http.post<RolUserListDto>(this.baseUrl, payload);
  }

  update(id: number, payload: RolUserUpdatePayload): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, payload);
  }

  deleteLogical(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

}
