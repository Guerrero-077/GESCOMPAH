import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment.development';
import { User } from '../../../shared/models/user.model';
import { LoginModel } from '../../../features/auth/models/login.models';
import { RegisterModel } from '../../../features/auth/models/register.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private urlBase = environment.apiURL + '/Auth/';

  constructor() { }

  Register(Objeto: RegisterModel): Observable<any> {
    return this.http.post<any>(this.urlBase + 'Register', Objeto);
  }

  Login(Objeto: LoginModel): Observable<any> {
    return this.http.post<any>(this.urlBase + 'Login', Objeto);
  }

  GetMe(): Observable<User> {
    return this.http.get<User>(this.urlBase + 'me');
  }
}
