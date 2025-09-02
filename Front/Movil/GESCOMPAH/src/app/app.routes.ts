// app.routes.ts
import { Routes } from '@angular/router';
import { MenuComponent } from './shared/menu/menu.component';

export const routes: Routes = [
  {
    path: '',
    component: MenuComponent,
    children: [
      {
        path: '',
        redirectTo: 'home',
        pathMatch: 'full',
      },
      {
        path: 'home',
        loadComponent: () =>
          import('./feature/home/home.component').then(
            (m) => m.HomeComponent
          ),
      },
      {
        path: 'establishment',
        loadComponent: () =>
          import('./feature/establishment/establishment.component').then(
            (m) => m.EstablishmentComponent
          ),
      },
      // Agrega más children para cada feature
    ],
  },
];
