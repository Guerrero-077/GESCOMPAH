import { Routes } from '@angular/router';
import { ContractComponent } from './contract.component';

export const routes: Routes = [
  {
    path: 'tabs',
    component: ContractComponent,
    children: [
      {
        path: 'tab1',
        loadComponent: () =>
          import('../contract/contract.component').then((m) => m.ContractComponent),
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
