import { Injectable } from '@angular/core';
import { GenericService } from '../../../../core/service/generic/generic.service';
import { AppointmentListComponent } from '../../pages/appointment-list/appointment-list.component';
import { AppointmentCreateModel, AppointmentSelect, AppointmentUpdateModel } from '../../models/appointment.models';

@Injectable({
  providedIn: 'root'
})
export class AppointmentService extends GenericService<AppointmentSelect, AppointmentCreateModel, AppointmentUpdateModel> {
  protected override resource = 'appointment';

}

export function FormAppointmentComponent(FormAppointmentComponent: any, arg1: { width: string; data: null; }) {
  throw new Error('Function not implemented.');
}
