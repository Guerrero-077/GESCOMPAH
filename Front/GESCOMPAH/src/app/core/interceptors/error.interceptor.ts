import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandlerFn,
  HttpInterceptorFn,
  HttpRequest
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { SweetAlertService } from '../../shared/Services/sweet-alert/sweet-alert.service'
import { Router } from '@angular/router';


export const errorInterceptor: HttpInterceptorFn = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn
): Observable<HttpEvent<unknown>> => {
  const sweetAlert = inject(SweetAlertService);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const currentUrl = router.url;
      const isAuthEndpoint = req.url.includes('/Auth/Refresh-Token') || req.url.includes('/Auth/me') || req.url.includes('/Auth/Login') || req.url.includes('/Auth/Register');
      const isPublicRoute = currentUrl === '/' || currentUrl.startsWith('/Auth');
      const suppress = isAuthEndpoint || isPublicRoute;

      // Evitar toasts en 401: el auth interceptor gestionará refresh/logout
      if (error.status === 401) {
        return throwError(() => error);
      }

      if (error.status === 403) {
        if (!suppress) {
          sweetAlert.showNotification('Acceso denegado', 'No tienes permisos para realizar esta acción.', 'warning');
        }
        return throwError(() => error);
      }

      if (suppress) {
        return throwError(() => error);
      }

      let errorMessage = 'Ocurrió un error inesperado';

      if (error.error instanceof ErrorEvent) {
        // Error del lado del cliente
        errorMessage = `Error: ${error.error.message}`;
      } else {
        // Error del lado del servidor
        errorMessage = (error.error && (error.error.message || error.error.title)) 
          ? (error.error.message || error.error.title)
          : `Error Código: ${error.status}, Mensaje: ${error.message}`;
      }

      sweetAlert.showNotification('Error', errorMessage, 'error');

      return throwError(() => error);
    })
  );
};
