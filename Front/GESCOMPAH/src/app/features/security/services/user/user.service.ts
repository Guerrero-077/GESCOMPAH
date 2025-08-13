// services/user.service.ts
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { UserListDto, UserCreateDto, UserUpdatePayload } from '../../models/user.models';

@Injectable({ providedIn: 'root' })
export class UserService {
  private readonly apiUrl = `${environment.apiURL}/user`;

  constructor(private http: HttpClient) { }

  getUsers(): Observable<UserListDto[]> {
    return this.http.get<UserListDto[]>(this.apiUrl);
  }

  // Tu back devuelve SelectDto para GetById:
  getUserById(id: number): Observable<UserListDto> {
    return this.http.get<UserListDto>(`${this.apiUrl}/${id}`);
  }

  createUser(dto: UserCreateDto): Observable<any> {
    return this.http.post<any>(this.apiUrl, dto);
  }

  updateUser(id: number, payload: UserUpdatePayload) {
    return this.http.put<any>(`${this.apiUrl}/${id}`, payload);
  }


  deleteUser(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
