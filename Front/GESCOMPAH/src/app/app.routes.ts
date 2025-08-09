import { Routes } from '@angular/router';
import { DashboardComponent } from './features/dashboard/Pages/dashboard/dashboard-component/dashboard-component';
import { EstablishmentsListComponent } from './features/establishments/components/establishments-list/establishments-list.component';
import { SquaresEstablishmentsComponent } from './features/establishments/pages/squares-establishments/squares-establishments.component';
import { TenantsListComponent } from './features/tenants/pages/tenants-list/tenants-list.component';
import { ModelSecurityComponent } from './features/security/pages/model-security/model-security.component';
import { RoleComponent } from './features/security/components/role/role.component';
import { FormComponent } from './features/security/components/form/form.component';
import { LayoutComponent } from './layout/layout.component';
import { CityComponent } from './features/setting/components/city/city.component';
import { DepartmentComponent } from './features/setting/components/department/department.component';

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
      { path: 'dashboard', component: DashboardComponent },
      { path: 'main', component: SquaresEstablishmentsComponent },
      { path: 'Establishment', component: EstablishmentsListComponent },
      { path: 'tenants', component: TenantsListComponent },
      { path: 'mainSistemSegurity', component: ModelSecurityComponent },
      { path: 'city', component: CityComponent },
      // { path: 'mainConfig', component: MainConfig },
      { path: 'departmentCities', component: DepartmentComponent },
      { path: 'rol', component: RoleComponent },
      { path: 'form', component: FormComponent },

    ],
    data: { role: 'Administrador' },
  },


];
