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
import { Router } from '@angular/router';
import { SweetAlertService } from '../../shared/Services/sweet-alert/sweet-alert.service';

export const errorInterceptor: HttpInterceptorFn = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn
): Observable<HttpEvent<unknown>> => {
  const sweetAlert = inject(SweetAlertService);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const currentUrl = router.url;

      // Evitar mostrar notificaciones en rutas públicas o endpoints de auth
      const isAuthEndpoint = req.url.includes('/Auth/Refresh-Token')
        || req.url.includes('/Auth/me')
        || req.url.includes('/Auth/Login')
        || req.url.includes('/Auth/Register');
      const isPublicRoute = currentUrl === '/' || currentUrl.startsWith('/Auth');
      const suppress = isAuthEndpoint || isPublicRoute;

      // No mostrar notificaciones en endpoints de auth
      if (error.status === 401) {
        return throwError(() => error);
      }

      // Mostrar toast para 403 si no estamos en rutas públicas
      if (error.status === 403 && !suppress) {
        sweetAlert.showNotification(
          'Acceso denegado',
          'No tienes permisos para realizar esta acción.',
          'warning'
        );
        return throwError(() => error);
      }

      if (suppress) {
        return throwError(() => error);
      }

      // Manejo de errores tipo ProblemDetails
      let errorMessage = 'Ocurrió un error inesperado.';

      if (error.error instanceof ErrorEvent) {
        // Error del lado del cliente
        errorMessage = `Error: ${error.error.message}`;
      } else {
        const problem = error.error;
        if (typeof problem === 'object') {
          if (problem.detail) {
            errorMessage = problem.detail;
          } else if (problem.title && !problem.errors) {
            errorMessage = problem.title;
          } else if (problem.errors) {
            const campos = Object.keys(problem.errors);
            if (campos.length > 0) {
              const primerCampo = campos[0];
              const mensajes = problem.errors[primerCampo];
              if (mensajes?.length > 0) {
                errorMessage = mensajes[0];
              }
            }
          }
        } else if (typeof error.error === 'string') {
          errorMessage = error.error;
        } else if (error.message) {
          errorMessage = error.message;
        }
      }

      sweetAlert.showNotification('Error', errorMessage, 'error');
      return throwError(() => error);
    })
  );
};
