import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { from, Observable, switchMap, tap } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { LoginModel } from '../../../feature/auth/models/login.models';
import { RegisterModel } from '../../../feature/auth/models/register.models';
import { User } from '../../../shared/models/user.model';
import { ChangePasswordDto } from '../../models/ChangePassword.models';
import { UserStore } from '../permission/User.Store';
import { CapacitorHttp, HttpResponse } from '@capacitor/core';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly urlBase = environment.apiURL + '/auth/';

  constructor(
    private router: Router,
    private userStore: UserStore
  ) {}

  private async buildHeaders(): Promise<Record<string, string>> {
    return {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
    };
  }

  Register(obj: RegisterModel): Observable<any> {
    return from(
      this.buildHeaders().then(headers =>
        CapacitorHttp.post({
          url: this.urlBase + 'register',
          data: obj,
          headers
        })
      )
    );
  }

  Login(obj: LoginModel): Observable<User> {
    return from(
      this.buildHeaders().then(headers =>
        CapacitorHttp.post({
          url: this.urlBase + 'login',
          data: obj,
          headers
        })
      )
    ).pipe(
      switchMap((response: HttpResponse) => {
        if (response.status >= 200 && response.status < 300) {
          return this.GetMe();
        } else {
          throw new HttpErrorResponse({
            status: response.status,
            statusText: 'Login failed', // ⚠️ CapacitorHttp no tiene `statusText`
            url: this.urlBase + 'login',
            error: response.data || { message: 'Credenciales inválidas' }
          });
        }
      })
    );
  }

  GetMe(): Observable<User> {
    return from(
      this.buildHeaders().then(headers =>
        CapacitorHttp.get({
          url: this.urlBase + 'me',
          headers
        })
      )
    ).pipe(
      tap(response => {
        const user = response.data as User;
        console.log('[GET /auth/me] user:', user);
        this.userStore.set(user);
      }),
      switchMap(response => from([response.data as User]))
    );
  }

  logout(): Observable<any> {
    return from(
      this.buildHeaders().then(headers =>
        CapacitorHttp.post({
          url: this.urlBase + 'logout',
          data: {},
          headers
        })
      )
    ).pipe(
      tap(() => {
        this.userStore.clear();
        this.router.navigate(['/']);
      })
    );
  }

  RefreshToken(): Observable<User> {
    return from(
      this.buildHeaders().then(headers =>
        CapacitorHttp.post({
          url: this.urlBase + 'refresh',
          data: {},
          headers
        })
      )
    ).pipe(
      switchMap((response: HttpResponse) => {
        if (response.status >= 200 && response.status < 300) {
          return this.GetMe();
        } else {
          throw new HttpErrorResponse({
            status: response.status,
            statusText: 'Refresh token failed',
            url: this.urlBase + 'refresh',
            error: response.data || { message: 'Token inválido' }
          });
        }
      })
    );
  }

  ChangePassword(dto: ChangePasswordDto): Observable<any> {
    return from(
      this.buildHeaders().then(headers =>
        CapacitorHttp.post({
          url: this.urlBase + 'change-password',
          data: dto,
          headers
        })
      )
    );
  }
}
