import { Routes } from "@angular/router";
import { AppointmentListComponent } from "./pages/appointment-list/appointment-list.component";


export const APPOINTMENT_ROUTES: Routes = [
  {
    path: '',
    component: AppointmentListComponent,
    data: {
      header: {
        title: 'Citas',
        description: 'Gestione las citas del Sistema - GESCOMPAH'
      }
    }
  }
]
