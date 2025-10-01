import { Routes } from '@angular/router';
import { HomeComponent } from '../home/home.component';
import { EstablishmentComponent } from './establishment.component';

export const routes: Routes = [
  {
    path: 'tabs',
    component: EstablishmentComponent,
    children: [
      {
        path: 'tab1',
        loadComponent: () =>
          import('../establishment/establishment.component').then((m) => m.EstablishmentComponent),
      },
      {
        path: '',
        redirectTo: 'tab1', // ✅ Relativo
        pathMatch: 'full',
      },
    ],
  },
  {
    path: '',
    redirectTo: '/tabs/tab1', // ✅ Absoluto para inicio de app
    pathMatch: 'full',
  },
];
