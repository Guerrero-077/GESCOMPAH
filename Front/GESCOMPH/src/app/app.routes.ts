import { Routes } from '@angular/router';
import { authGuard } from './core/security/guards/auth.guard';
import { publicGuard } from './core/security/guards/public.guard';
import { LandingComponent } from './features/public/pages/landing/landing.component';
import { LayoutComponent } from './layout/layout.component';
import { NotFoundComponent } from './shared/components/not-found/not-found.component';
import { EstablishmentListComponent } from './features/public/pages/establishment-list/establishment-list.component';

export const routes: Routes = [
  // Landing pública
  { path: '', pathMatch: 'full', component: LandingComponent, canActivate: [publicGuard], title: 'Inicio' },
  { path: 'establishments', pathMatch: 'full', component: EstablishmentListComponent, canActivate: [publicGuard], title: 'Establecimientos Disponibles' },

  // Área de autenticación (lazy)
  {
    path: 'auth',
    canActivate: [publicGuard],
    loadChildren: () => import('./features/auth-login/auth.routes').then(m => m.AUTH_ROUTES),
  },

  // Shell privado (asegura sesión + rehidratación /me)
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard], // aquí validas que hay user/menu; el guard puede devolver UrlTree a /auth/login o /dashboard
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },

      // A partir de aquí, cada feature lazy protegido por canMatch (no se carga si no hay permiso/ruta)
      {
        path: 'dashboard',
        // si quieres filtrar fino por permiso/ruta, usa canMatch especializado:
        // canMatch: [permissionMatchGuard],
        canMatch: [authGuard],
        loadChildren: () => import('./features/dashboard/dashboard.routes').then(m => m.DASHBOARD_ROUTES),
        title: 'Dashboard',
      },
      {
        path: 'establishments',
        canMatch: [authGuard], // o permissionMatchGuard si validas permisos específicos
        loadChildren: () => import('./features/establishments/establishments.routes').then(m => m.ESTABLISHMENTS_ROUTES),
        title: 'Establecimientos',
      },
      {
        path: 'tenants',
        canMatch: [authGuard],
        loadChildren: () => import('./features/tenants/tenants.routes').then(m => m.TENANTS_ROUTES),
        title: 'Arrendatarios',
      },
      {
        path: 'contracts',
        canMatch: [authGuard],
        loadChildren: () => import('./features/contracts/contracts.routes').then(m => m.CONTRACTS_ROUTES),
        title: 'Contratos',
      },
      {
        path: 'appointment',
        canMatch: [authGuard],
        loadChildren: () => import('./features/appointment/appointment.routes').then(m => m.APPOINTMENT_ROUTES),
        title: 'Citas',
      },
      {
        path: 'security',
        canMatch: [authGuard],
        loadChildren: () => import('./features/security/security.routes').then(m => m.SECURITY_ROUTES),
        title: 'Seguridad',
      },
      {
        path: 'settings',
        canMatch: [authGuard],
        loadChildren: () => import('./features/setting/setting.routes').then(m => m.SETTING_ROUTES),
        title: 'Configuración',
      },
    ],
  },

  // 404 SIEMPRE al final
  { path: '**', component: NotFoundComponent, title: 'No encontrado' },
];
