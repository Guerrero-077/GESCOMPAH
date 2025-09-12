import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError, EMPTY } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthService } from '../service/auth/auth.service';
import { UserStore } from '../service/permission/User.Store';
import { SweetAlertService } from '../../shared/Services/sweet-alert/sweet-alert.service';


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

      // Cualquier error al llamar al endpoint de refresh invalida la sesión
      if (isApiRequest && isRefreshEndpoint && isHttp) {
        userStore.clear();
        // Aviso suave al usuario
        sweet.showNotification('Sesión expirada', 'Tu sesión expiró, vuelve a iniciar sesión.', 'info');
        router.navigate(['/']);
        return EMPTY;
      }

      // Para 401 en otros endpoints de API, intentamos refresh y reintentamos
      if (isHttp && status === 401 && isApiRequest && !isRefreshEndpoint) {
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
