import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../../environments/environment.development';
import { User } from '../../../shared/models/user.model';
import { LoginModel } from '../../../features/auth-login/models/login.models';
import { RegisterModel } from '../../../features/auth-login/models/register.models';
import { PermissionService } from '../permission/permission.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private permissionService = inject(PermissionService);
  private router = inject(Router);
  private urlBase = environment.apiURL + '/Auth/';

  constructor() { }

  Register(Objeto: RegisterModel): Observable<any> {
    return this.http.post<any>(this.urlBase + 'Register', Objeto);
  }

  Login(Objeto: LoginModel): Observable<any> {
    return this.http.post<any>(this.urlBase + 'Login', Objeto).pipe(
      tap(() => {
        this.GetMe().subscribe(user => {
          this.permissionService.setUserProfile(user);
        });
      })
    );
  }

  GetMe(): Observable<User> {
    return this.http.get<User>(this.urlBase + 'me').pipe(
      tap(user => {
        this.permissionService.setUserProfile(user);
      })
    );
  }

  logout(): Observable<any> {
    return this.http.post(this.urlBase + 'Logout', {}).pipe(
      tap(() => {
        this.permissionService.setUserProfile(null);
        this.router.navigate(['/']);
      })
    );
  }
}
