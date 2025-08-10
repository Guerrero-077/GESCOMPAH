import { Routes } from '@angular/router';
import { SquaresEstablishmentsComponent } from './pages/squares-establishments/squares-establishments.component';
import { EstablishmentsListComponent } from './components/establishments-list/establishments-list.component';

export const ESTABLISHMENTS_ROUTES: Routes = [
  { path: 'main', component: SquaresEstablishmentsComponent }
];
