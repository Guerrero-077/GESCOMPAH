import { Routes } from '@angular/router';
import { LayoutComponent } from './layout/layout.component';
import { authGuard } from './core/guards/auth.guard';
import { publicGuard } from './core/guards/public.guard';
import { NotFoundComponent } from './shared/components/not-found/not-found.component';
import { LandingComponent } from './features/public/pages/landing/landing.component';


export const routes: Routes = [
  // Set the new landing page as the default route
  { path: '', component: LandingComponent, canActivate: [publicGuard] },
  {
    path: 'Auth',
    canActivate: [publicGuard], // Guardian para rutas públicas
    loadChildren: () =>
      import('./features/auth-login/auth.routes').then((m) => m.AUTH_ROUTES),
  },
  {
    path: 'Admin',
    component: LayoutComponent,
    canActivate: [authGuard], // Guardian para rutas privadas
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadChildren: () =>
          import('./features/dashboard/dashboard.routes').then((m) => m.DASHBOARD_ROUTES),
      },
      {
        path: 'establishments',
        loadChildren: () =>
          import('./features/establishments/establishments.routes').then((m) => m.ESTABLISHMENTS_ROUTES),
      },
      {
        path: 'tenants',
        loadChildren: () =>
          import('./features/tenants/tenants.routes').then((m) => m.TENANTS_ROUTES),
      },
      {
        path: 'security',
        loadChildren: () =>
          import('./features/security/security.routes').then((m) => m.SECURITY_ROUTES),
      },

    {
        path: 'settings',
        loadChildren: () =>
          import('./features/setting/setting.routes').then((m) => m.SETTING_ROUTES),
      },

    ],
    data: { role: 'Administrador' },
  },
  // Wildcard route for 404 page - MUST BE THE LAST ROUTE
  { path: '**', component: NotFoundComponent },

];