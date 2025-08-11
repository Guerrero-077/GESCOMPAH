import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../service/auth/auth.service';
import { PermissionService } from '../service/permission/permission.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const permissionService = inject(PermissionService);

  const isApiRequest = req.url.startsWith(environment.apiURL);

  // Siempre enviamos credenciales (cookies) en llamadas a la API
  if (isApiRequest) {
    req = req.clone({ withCredentials: true });
  }

  return next(req).pipe(
    catchError((error) => {
      if (error instanceof HttpErrorResponse && error.status === 401 && isApiRequest) {
        // Intentar refrescar el token
        return authService.RefreshToken().pipe(
          switchMap(() => {
            // Reintentamos la petición original
            return next(req);
          }),
          catchError((refreshError) => {
            // Si falla el refresh, limpiar sesión sin redirigir para no interrumpir rutas públicas
            permissionService.setUserProfile(null);
            return throwError(() => refreshError);
          })
        );
      }
      return throwError(() => error);
    })
  );
};
