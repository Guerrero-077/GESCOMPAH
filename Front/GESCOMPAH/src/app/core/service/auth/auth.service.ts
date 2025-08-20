import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, switchMap, tap } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { User } from '../../../shared/models/user.model';
import { LoginModel } from '../../../features/auth-login/models/login.models';
import { RegisterModel } from '../../../features/auth-login/models/register.models';
import { PermissionService } from '../permission/permission.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private permissionService = inject(PermissionService);
  private router = inject(Router);

  // ⬅️ Alineado a kebab/lower
  private urlBase = environment.apiURL + '/auth/';

  Register(obj: RegisterModel): Observable<any> {
    return this.http.post<any>(this.urlBase + 'register', obj, { withCredentials: true });
  }


  // Login -> luego /me para poblar el perfil
  Login(obj: LoginModel): Observable<User> {
    return this.http.post<any>(this.urlBase + 'login', obj, { withCredentials: true }).pipe(
      switchMap(() => this.GetMe())
    );
  }

  // Fuente de verdad del perfil
  GetMe(): Observable<User> {
    return this.http.get<User>(this.urlBase + 'me', { withCredentials: true }).pipe(
      tap(user => this.permissionService.setUserProfile(user))
    );
  }

  logout(): Observable<any> {
    return this.http.post(this.urlBase + 'logout', {}, { withCredentials: true }).pipe(
      tap(() => {
        this.permissionService.setUserProfile(null);
        this.router.navigate(['/']); // ⬅️ ruta del front en minúsculas
      })
    );
  }

  // Refresh -> luego /me para mantener el estado en memoria
  RefreshToken(): Observable<User> {
    return this.http.post<any>(this.urlBase + 'refresh', {}, { withCredentials: true }).pipe(
      switchMap(() => this.GetMe())
    );
  }
}
