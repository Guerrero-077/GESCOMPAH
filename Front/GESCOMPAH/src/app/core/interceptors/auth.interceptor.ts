// src/app/core/interceptors/auth.interceptor.ts
import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthService } from '../service/auth/auth.service';
import { PermissionService } from '../service/permission/permission.service';

// Lee una cookie por nombre (para CSRF)
function getCookie(name: string): string | null {
  const m = document.cookie.match(new RegExp('(?:^|; )' + name + '=([^;]*)'));
  return m ? decodeURIComponent(m[1]) : null;
}

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const permissionService = inject(PermissionService);

  const isApiRequest = req.url.startsWith(environment.apiURL);
  const isRefreshEndpoint = /\/auth\/refresh$/i.test(req.url);

  if (isApiRequest) {
    // ⚠️ Asegúrate que este nombre coincida con CookieSettings.CsrfCookieName en tu back (p.ej. "XSRF-TOKEN")
    const csrfCookie = getCookie('XSRF-TOKEN');

    // Para peticiones a tu API:
    // - enviamos cookies (access/refresh/csrf) con withCredentials
    // - y mandamos el encabezado X-XSRF-TOKEN con el valor de la cookie (double-submit)
    req = req.clone({
      withCredentials: true,
      setHeaders: csrfCookie ? { 'X-XSRF-TOKEN': csrfCookie } : {}
    });
  }

  return next(req).pipe(
    catchError((error) => {
      // Si el access token expiró (401), intentamos refrescar de forma transparente
      if (error instanceof HttpErrorResponse && error.status === 401 && isApiRequest && !isRefreshEndpoint) {
        return authService.RefreshToken().pipe(
          // Si el refresh fue OK, repetimos la petición original
          switchMap(() => next(req)),
          // Si el refresh también falla, limpiamos el perfil y propagamos el error
          catchError((refreshError) => {
            permissionService.setUserProfile(null);
            return throwError(() => refreshError);
          })
        );
      }
      return throwError(() => error);
    })
  );
};
