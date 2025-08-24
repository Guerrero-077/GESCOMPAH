import { ApplicationConfig, inject } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { AuthService } from './core/service/auth/auth.service';
import { catchError, of, firstValueFrom } from 'rxjs';
import { provideAppInitializer } from '@angular/core';

// Función de inicialización
export function initializeApp() {
  const authService = inject(AuthService);
  return firstValueFrom(
    authService.GetMe().pipe(
      catchError(() => of(null))
    )
  );
}



export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(
      withFetch(),
      withInterceptors([
        errorInterceptor,
        authInterceptor
      ])
    ),
    provideAppInitializer(initializeApp)
  ]
};
