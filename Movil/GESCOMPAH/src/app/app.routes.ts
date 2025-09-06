// app.routes.ts
import { Routes } from '@angular/router';
import { MenuComponent } from './shared/menu/menu.component';

export const routes: Routes = [
  { path: 'auth',
    loadChildren: () => import('./feature/auth/auth.routes').then(m => m.AUTH_ROUTES),
  },
  {
    path: '',
    component: MenuComponent,
    children: [
      {
        path: '',
        redirectTo: 'auth',
        pathMatch: 'full',
      },

      {
        path: 'auth',
        // canActivate: [publicGuard],
        loadChildren: () => import('./feature/auth/auth.routes').then(m => m.AUTH_ROUTES),
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
      {
        path: 'contract',
        loadComponent: () =>
          import('./feature/contract/contract.component').then(
            (m) => m.ContractComponent
          ),
      },
      // Agrega más children para cada feature
    ],
  },
];
