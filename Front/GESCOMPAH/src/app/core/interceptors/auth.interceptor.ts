import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../service/auth/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const isApiRequest = req.url.startsWith(environment.apiURL);

  // Siempre enviamos credenciales (cookies)
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
            // Si falla el refresh, redirigir al login
            authService.logout().subscribe();
            router.navigate(['/login']);
            return throwError(() => refreshError);
          })
        );
      }
      return throwError(() => error);
    })
  );
};
