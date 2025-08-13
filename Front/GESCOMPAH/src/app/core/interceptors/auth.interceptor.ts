import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthService } from '../service/auth/auth.service';
import { PermissionService } from '../service/permission/permission.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const permissionService = inject(PermissionService);

  const isApiRequest = req.url.startsWith(environment.apiURL);
  const isRefreshEndpoint = /\/auth\/refresh$/i.test(req.url);

  if (isApiRequest) {
    req = req.clone({ withCredentials: true });
  }

  return next(req).pipe(
    catchError((error) => {
      if (error instanceof HttpErrorResponse && error.status === 401 && isApiRequest && !isRefreshEndpoint) {
        // Intentar refrescar SOLO si no estamos en el propio endpoint de refresh
        return authService.RefreshToken().pipe(
          switchMap(() => next(req)),
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
