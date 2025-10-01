import { Routes } from '@angular/router';
import { HomeComponent } from '../home/home.component';

export const routes: Routes = [
  {
    path: 'tabs',
    component: HomeComponent,
    children: [
      {
        path: 'tab1',
        loadComponent: () =>
          import('../home/home.component').then((m) => m.HomeComponent),
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
