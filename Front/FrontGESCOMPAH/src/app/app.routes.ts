import { Routes } from '@angular/router';
import { LoginComponent } from './features/Auth/pages/login/login-component/login-component';

export const routes: Routes = [
  { path: '', redirectTo: 'Login', pathMatch: 'full' },
  { path: 'Login', component: LoginComponent },
  {
    path: 'Auth',
    loadChildren: () =>
      import('./features/Auth/auth.routes').then((m) => m.AUTH_ROUTES),
  },
  {
    path: 'Home',
    loadChildren: () =>
      import('./features/Dashboard/dashboard.routes').then((m) => m.DASHBOARD_ROUTES),
  },
];
