import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthService } from '../service/auth/auth.service';
import { UserStore } from '../service/permission/User.Store';


function getCookie(name: string): string | null {
  const m = document.cookie.match(new RegExp('(?:^|; )' + name + '=([^;]*)'));
  return m ? decodeURIComponent(m[1]) : null;
}

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const userStore = inject(UserStore);

  const isApiRequest = req.url.startsWith(environment.apiURL);
  const isRefreshEndpoint = /\/auth\/refresh$/i.test(req.url);

  if (isApiRequest) {
    const csrfCookie = getCookie('XSRF-TOKEN');

    req = req.clone({
      withCredentials: true,
      setHeaders: csrfCookie ? { 'X-XSRF-TOKEN': csrfCookie } : {}
    });
  }

  return next(req).pipe(
    catchError((error) => {
      if (error instanceof HttpErrorResponse && error.status === 401 && isApiRequest && !isRefreshEndpoint) {
        return authService.RefreshToken().pipe(
          switchMap(() => next(req)),
          catchError((refreshError) => {
            userStore.clear(); // ✅ Actualizado
            return throwError(() => refreshError);
          })
        );
      }
      return throwError(() => error);
    })
  );
};
