import { Routes } from '@angular/router';
import { LayoutComponent } from './layout/layout.component';
import { authGuard } from './core/guards/auth.guard';
import { publicGuard } from './core/guards/public.guard';
import { NotFoundComponent } from './shared/components/not-found/not-found.component';
import { LandingComponent } from './features/public/pages/landing/landing.component';

export const routes: Routes = [
  // Landing pública: si el usuario ya está autenticado, publicGuard puede redirigirlo al dashboard
  { path: '', pathMatch: 'full', component: LandingComponent, canMatch: [publicGuard], title: 'Inicio' },

  // Área de autenticación (login/registro/recuperación, etc.)
  {
    path: 'auth',
    canMatch: [publicGuard], // evita cargar el bundle de auth si ya está logueado
    loadChildren: () => import('./features/auth-login/auth.routes').then(m => m.AUTH_ROUTES),
  },

  // Área privada
  {
    path: 'admin',
    component: LayoutComponent,
    // canMatch: [authGuard], // evita cargar cualquier child si no hay sesión
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },

      {
        path: 'dashboard',
        loadChildren: () => import('./features/dashboard/dashboard.routes').then(m => m.DASHBOARD_ROUTES),
        title: 'Dashboard',
      },
      {
        path: 'establishments',
        loadChildren: () => import('./features/establishments/establishments.routes').then(m => m.ESTABLISHMENTS_ROUTES),
        title: 'Establecimientos',
        // data: { roles: ['Administrador'] }, // si necesitas filtrar por rol/permiso con otro guard
      },
      {
        path: 'tenants',
        loadChildren: () => import('./features/tenants/tenants.routes').then(m => m.TENANTS_ROUTES),
        title: 'Arrendatarios',
      },
      {
        path: 'security',
        loadChildren: () => import('./features/security/security.routes').then(m => m.SECURITY_ROUTES),
        title: 'Seguridad',
        // data: { permissions: ['AdministrarSeguridad'] },
      },
      {
        path: 'settings',
        loadChildren: () => import('./features/setting/setting.routes').then(m => m.SETTING_ROUTES),
        title: 'Configuración',
      },
    ],
  },

  // 404 SIEMPRE al final
  { path: '**', component: NotFoundComponent, title: 'No encontrado' },
];
