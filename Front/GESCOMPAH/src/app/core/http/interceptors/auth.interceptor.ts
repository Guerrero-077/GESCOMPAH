import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError, EMPTY } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { AuthService } from '../../security/services/auth/auth.service';
import { UserStore } from '../../security/services/permission/User.Store';
import { AuthEventsService } from '../../security/services/auth/auth-events.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const userStore = inject(UserStore);
  const authEvents = inject(AuthEventsService);

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
        return EMPTY;
      }

      // 401 en API
      if (isHttp && status === 401 && isApiRequest && !isRefreshEndpoint) {
        if (isLoginEndpoint) {
          return throwError(() => error);
        }

        // Intentar refresh
        return authService.RefreshToken().pipe(
          switchMap(() => next(req)),
          catchError(() => {
            userStore.clear();
            authEvents.sessionExpired();
            return EMPTY;
          })
        );
      }

      return throwError(() => error);
    })
  );
};
