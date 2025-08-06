import { Routes } from '@angular/router';
import { LayoutComponent } from './features/layout.component';
import { DashboardComponent } from './features/dashboard/Pages/dashboard/dashboard-component/dashboard-component';
import { EstablishmentFormComponent } from './features/Establishment/Pages/Form/establishment-form-component/establishment-form-component';
import { ListEstablishmentComponent } from './features/Establishment/Pages/list-establishment-component/list-establishment-component';
import { RolComponent } from './features/ModelSecurity/Pages/Rol/rol-component/rol-component';
import { FormComponent } from './features/ModelSecurity/Pages/Form/form-component/form-component';
import { PermissionComponent } from './features/ModelSecurity/Pages/Permission/permission-component/permission-component';

export const routes: Routes = [
  { path: '', redirectTo: 'Auth/login', pathMatch: 'full' },
  // { path: '', redirectTo: 'Admin', pathMatch: 'full' },
  {
    path: 'Auth',
    loadChildren: () =>
      import('./features/Auth/auth.routes').then((m) => m.AUTH_ROUTES),
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
      { path: 'Establishment', component: ListEstablishmentComponent },
    ],
    data: { role: 'Administrador' },
  },
  { path: 'Rol', component: RolComponent },
  { path: 'Form', component: FormComponent },
  { path: 'Permission', component: PermissionComponent },
];
