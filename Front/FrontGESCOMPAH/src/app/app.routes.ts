import { Routes } from '@angular/router';
import { LoginComponent } from './features/Auth/pages/login/login-component/login-component';

export const routes: Routes = [
  { path: '', redirectTo: 'Auth/login', pathMatch: 'full' },
  {
    path: 'Auth',
    loadChildren: () =>
      import('./features/Auth/auth.routes').then((m) => m.AUTH_ROUTES),
  },
  {
    path: 'Establishment',
    loadChildren: () =>
      import('./features/Establishment/establishment.routes').then((m) => m.ESTABLISHMENT_ROUTES),
  },
];
