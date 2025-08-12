import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { User, UserCreateDto, UserUpdateDto, UserListDto } from '../../models/user.models';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly apiUrl = `${environment.apiURL}/user`;

  constructor(private http: HttpClient) { }

  getUsers(): Observable<UserListDto[]> {
    return this.http.get<UserListDto[]>(this.apiUrl);
  }

  getUserById(id: number): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/${id}`);
  }

  createUser(dto: UserCreateDto): Observable<User> {
    return this.http.post<User>(this.apiUrl, dto);
  }

  updateUser(id: number, dto: UserUpdateDto): Observable<User> {
    return this.http.put<User>(`${this.apiUrl}/${id}`, dto);
  }


  deleteUser(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
