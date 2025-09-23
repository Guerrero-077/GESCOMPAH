export interface AppointmentBaseModel {
  description: string;
  observation?: string;
  requestDate: Date;
  dateTimeAssigned: Date;
  establishmentId: number;
}

export interface AppointmentCreateModel extends AppointmentBaseModel {
  firstName: string;
  lastName: string;
  document: string;
  adrress: string;
  phone: string;
  cityId: number;
}


export interface AppointmentUpdateModel extends AppointmentBaseModel {
  id: number;
  active: boolean;
}

export interface AppointmentSelect extends AppointmentBaseModel
{
  id: number;
  personId: number;
  personName: string;
  establishmentName: string;
  phone: string;
  active: boolean;
}
