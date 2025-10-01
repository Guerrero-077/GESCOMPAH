import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError, EMPTY, shareReplay, finalize } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { AuthService } from '../../security/services/auth/auth.service';
import { UserStore } from '../../security/services/permission/User.Store';
import { AuthEventsService } from '../../security/services/auth/auth-events.service';

// Evita múltiples llamadas concurrentes a /auth/refresh (single-flight)
let refreshToken$: ReturnType<AuthService['RefreshToken']> | null = null;

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const userStore = inject(UserStore);
  const authEvents = inject(AuthEventsService);
  const router = inject(Router);

  const isApiRequest = req.url.startsWith(environment.apiURL);
  const isRefreshEndpoint = /\/auth\/refresh$/i.test(req.url);
  const isLoginEndpoint = /\/auth\/login$/i.test(req.url);

  return next(req).pipe(
    catchError((error) => {
      const isHttp = error instanceof HttpErrorResponse;
      const status = isHttp ? error.status : 0;

      // Si falla el refresh → sesión expirada
      if (isApiRequest && isRefreshEndpoint && isHttp) {
        userStore.clear();
        authEvents.sessionExpired();
        try { router.navigate(['/auth/login']); } catch {}
        return throwError(() => ({ __authExpired: true, status: 401 }));
      }

      // 401 en API
      if (isHttp && status === 401 && isApiRequest && !isRefreshEndpoint) {
        if (isLoginEndpoint) {
          return throwError(() => error);
        }

        // Intentar refresh con single-flight: compartir 1 llamada para todos los 401 simultáneos
        if (!refreshToken$) {
          refreshToken$ = authService.RefreshToken().pipe(
            shareReplay(1),
            finalize(() => { refreshToken$ = null; })
          );
        }

        return refreshToken$.pipe(
          switchMap(() => next(req)),
          catchError(() => {
            userStore.clear();
            authEvents.sessionExpired();
            try { router.navigate(['/auth/login']); } catch {}
            return throwError(() => ({ __authExpired: true, status: 401 }));
          })
        );
      }

      return throwError(() => error);
    })
  );
};
