import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError, EMPTY } from 'rxjs';

import { SweetAlertService } from '../../../shared/Services/sweet-alert/sweet-alert.service';
import { environment } from '../../../../environments/environment';
import { AuthService } from '../../security/services/auth/auth.service';
import { UserStore } from '../../security/services/permission/User.Store';

function getCookie(name: string): string | null {
  const m = document.cookie.match(new RegExp('(?:^|; )' + name + '=([^;]*)'));
  return m ? decodeURIComponent(m[1]) : null;
}

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const userStore = inject(UserStore);
  const router = inject(Router);
  const sweet = inject(SweetAlertService);

  const isApiRequest = req.url.startsWith(environment.apiURL);
  const isRefreshEndpoint = /\/auth\/refresh$/i.test(req.url);
  const isLoginEndpoint = /\/auth\/login$/i.test(req.url);

  // Agrega cabecera CSRF si es necesario
  if (isApiRequest) {
    const csrfCookie = getCookie('XSRF-TOKEN');
    req = req.clone({
      withCredentials: true,
      setHeaders: csrfCookie ? { 'X-XSRF-TOKEN': csrfCookie } : {}
    });
  }

  return next(req).pipe(
    catchError((error) => {
      const isHttp = error instanceof HttpErrorResponse;
      const status = isHttp ? error.status : 0;

      // Si el error ocurre en /auth/refresh → cerrar sesión
      if (isApiRequest && isRefreshEndpoint && isHttp) {
        userStore.clear();
        sweet.showNotification('Sesión expirada', 'Tu sesión expiró, vuelve a iniciar sesión.', 'info');
        router.navigate(['/']);
        return EMPTY;
      }

      // Si el error es 401 en cualquier otro endpoint de API
      if (isHttp && status === 401 && isApiRequest && !isRefreshEndpoint) {
        if (isLoginEndpoint) {
          // NO refrescar token si es el login
          return throwError(() => error);
        }

        // Intentar refresh
        return authService.RefreshToken().pipe(
          switchMap(() => next(req)),
          catchError(() => {
            userStore.clear();
            sweet.showNotification('Sesión expirada', 'Tu sesión expiró, vuelve a iniciar sesión.', 'info');
            router.navigate(['/']);
            return EMPTY;
          })
        );
      }

      return throwError(() => error);
    })
  );
};
