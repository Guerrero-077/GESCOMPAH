import { ApplicationConfig, LOCALE_ID } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';

import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { provideCloudinaryLoader } from '@angular/common';
import { authInterceptor } from './core/http/interceptors/auth.interceptor';
import { errorInterceptor } from './core/http/interceptors/error.interceptor';
import { ngrokCredentialsInterceptor } from './core/http/interceptors/ngrok-credentials.interceptor';

import { provideAnimations } from '@angular/platform-browser/animations';
import { provideNativeDateAdapter, MAT_DATE_LOCALE } from '@angular/material/core';
import { environment } from '../environments/environment';

import localeEsCO from '@angular/common/locales/es-CO';
import { registerLocaleData } from '@angular/common';
import { csrfInterceptor } from './core/http/interceptors/csrf.Interceptor';
registerLocaleData(localeEsCO);

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),

    provideHttpClient(
      withFetch(),
      withInterceptors([
        csrfInterceptor,
        ...(environment.production ? [] : [ngrokCredentialsInterceptor]),
        authInterceptor,
        errorInterceptor
      ])
    ),

    provideAnimations(),
    provideNativeDateAdapter(),

    { provide: LOCALE_ID, useValue: 'es-CO' },

    { provide: MAT_DATE_LOCALE, useValue: 'es-CO' },

    provideCloudinaryLoader('https://res.cloudinary.com/dmbndpjlh/')
  ]
};
