import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, switchMap, tap } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { LoginModel } from '../../../features/auth-login/models/login.models';
import { RegisterModel } from '../../../features/auth-login/models/register.models';
import { User } from '../../../shared/models/user.model';
import { PermissionService } from '../permission/permission.service';
import { UserStore } from '../permission/User.Store';
import { ChangePasswordDto } from '../../models/ChangePassword.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private permissionService = inject(PermissionService);
  private router = inject(Router);
  private userStore = inject(UserStore);

  private urlBase = environment.apiURL + '/auth/';

  Register(obj: RegisterModel): Observable<any> {
    return this.http.post<any>(this.urlBase + 'register', obj, { withCredentials: true });
  }


  Login(obj: LoginModel): Observable<User> {
    return this.http.post<any>(this.urlBase + 'login', obj, { withCredentials: true }).pipe(
      switchMap(() => this.GetMe())
    );
  }


  GetMe(): Observable<User> {
    return this.http.get<User>(this.urlBase + 'me', { withCredentials: true }).pipe(
      tap(user => this.userStore.set(user))
    );
  }

  logout(): Observable<any> {
    return this.http.post(this.urlBase + 'logout', {}, { withCredentials: true }).pipe(
      tap(() => {
        this.userStore.clear();
        this.router.navigate(['/']);
      })
    );
  }

  RefreshToken(): Observable<User> {
    return this.http.post<any>(this.urlBase + 'refresh', {}, { withCredentials: true }).pipe(
      switchMap(() => this.GetMe())
    );
  }

  ChangePassword(dto: ChangePasswordDto): Observable<any> {
    return this.http.post(environment.apiURL + '/auth/change-password', dto, { withCredentials: true });
  }
}
