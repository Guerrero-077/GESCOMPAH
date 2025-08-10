import { APP_INITIALIZER, ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { AuthService } from './core/service/auth/auth.service';
import { catchError, of } from 'rxjs';

// Factory function that calls GetMe()
export function initializeApp(authService: AuthService) {
  return () => authService.GetMe().pipe(
    // If GetMe fails, catch the error and return a null observable
    // This prevents the app from crashing on startup if the user is not logged in
    catchError(() => of(null))
  );
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(
      withFetch(),
      withInterceptors([authInterceptor, errorInterceptor])
    ),
    // APP_INITIALIZER provider
    {
      provide: APP_INITIALIZER,
      useFactory: initializeApp,
      deps: [AuthService], // Dependency injection for the factory
      multi: true
    }
  ]
};
