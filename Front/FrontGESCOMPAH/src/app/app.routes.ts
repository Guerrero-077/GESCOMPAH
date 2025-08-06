import { Routes } from '@angular/router';
import { LoginComponent } from './features/Auth/pages/login/login-component/login-component';
import { LayoutComponent } from './features/layout.component';
import { ListEstablishmentComponent } from './features/Establishment/Pages/list-establishment-component/list-establishment-component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { PropiedadComponent } from './features/propiedad/propiedad.component';
import { ArrentadariosComponent } from './features/arrentadarios/arrentadarios.component';

export const routes: Routes = [
  // { path: '', redirectTo: 'Auth', pathMatch: 'full' },
  { path: '', redirectTo: 'Admin', pathMatch: 'full' },
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
      { path: 'dashboard' , component: DashboardComponent},
      { path: 'propiedades' , component: PropiedadComponent},
      { path: 'arrendatarios' , component: ArrentadariosComponent},
    ],
    data: { role: 'Administrador' },
  }
];
