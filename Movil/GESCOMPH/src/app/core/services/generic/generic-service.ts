import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { from, Observable, switchMap, tap } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { LoginModel } from '../../../feature/auth/models/login.models';
import { RegisterModel } from '../../../feature/auth/models/register.models';
import { User } from '../../../shared/models/user.model';
import { ChangePasswordDto } from '../../models/ChangePassword.models';
import { UserStore } from '../permission/User.Store';
import { CapacitorHttp } from '@capacitor/core';

function getCookie(name: string): string | null {
  const m = document.cookie.match(new RegExp('(?:^|; )' + name.replace(/[$()*+./?[\\\]^{|}-]/g, '\\$&') + '=([^;]*)'));
  return m ? decodeURIComponent(m[1]) : null;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly urlBase = environment.apiURL + '/auth/';

  constructor(
    private router: Router,
    private userStore: UserStore
  ) {}

  /** Obtiene headers con token CSRF si está disponible */
  private async buildHeaders(): Promise<Record<string, string>> {
    const token = getCookie('XSRF-TOKEN');
    return {
      'Content-Type': 'application/json',
      ...(token ? { 'X-XSRF-TOKEN': token } : {}),
    };
  }

  /** Registro de usuario */
  Register(obj: RegisterModel): Observable<any> {
    return from(this.buildHeaders().then(headers =>
      CapacitorHttp.post({
        url: this.urlBase + 'register',
        data: obj,
        headers,
      })
    ));
  }

  /** Login y recuperación de sesión */
  Login(obj: LoginModel): Observable<User> {
    return from(this.buildHeaders().then(headers =>
      CapacitorHttp.post({
        url: this.urlBase + 'login',
        data: obj,
        headers,
      })
    )).pipe(
      switchMap(() => this.GetMe())
    );
  }

  /** Consulta la sesión activa y almacena el usuario */
  GetMe(): Observable<User> {
    return from(this.buildHeaders().then(headers =>
      CapacitorHttp.get({
        url: this.urlBase + 'me',
        headers,
      })
    )).pipe(
      tap(response => {
        const user = response.data as User;
        console.log('[GET /auth/me] user:', user);
        this.userStore.set(user);
      }),
      switchMap(response => from([response.data as User]))
    );
  }

  /** Cierre de sesión */
  logout(): Observable<any> {
    return from(this.buildHeaders().then(headers =>
      CapacitorHttp.post({
        url: this.urlBase + 'logout',
        data: {},
        headers,
      })
    )).pipe(
      tap(() => {
        this.userStore.clear();
        this.router.navigate(['/']);
      })
    );
  }

  /** Renovación del token de sesión */
  RefreshToken(): Observable<User> {
    return from(this.buildHeaders().then(headers =>
      CapacitorHttp.post({
        url: this.urlBase + 'refresh',
        data: {},
        headers,
      })
    )).pipe(
      switchMap(() => this.GetMe())
    );
  }

  /** Cambio de contraseña del usuario */
  ChangePassword(dto: ChangePasswordDto): Observable<any> {
    return from(this.buildHeaders().then(headers =>
      CapacitorHttp.post({
        url: this.urlBase + 'change-password',
        data: dto,
        headers,
      })
    ));
  }
}
