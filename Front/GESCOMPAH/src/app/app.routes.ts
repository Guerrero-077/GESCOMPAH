import { Routes } from '@angular/router';
import { LayoutComponent } from './layout/layout.component';
import { CityComponent } from './features/setting/components/city/city.component';
import { DepartmentComponent } from './features/setting/components/department/department.component';
import { MainSettingsComponent } from './features/setting/pages/main-settings/main-settings.component';


export const routes: Routes = [
  { path: '', redirectTo: 'Auth/login', pathMatch: 'full' },
  // { path: '', redirectTo: 'Admin', pathMatch: 'full' },
  {
    path: 'Auth',
    loadChildren: () =>
      import('./features/auth-login/auth.routes').then((m) => m.AUTH_ROUTES),
  },
  // {
  //   path: 'Establishment',
  //   loadChildren: () =>
  //     import('./features/Establishment/establishment.routes').then((m) => m.ESTABLISHMENT_ROUTES),
  // },
  {
    path: 'Admin',
    component: LayoutComponent,
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


];