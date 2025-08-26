export interface AppointmentBaseModel {
  fullName: string;
  email: string;
  phone: string;
  description: string;
  requestDate: Date;
  dateTimeAssigned: Date;
  establishmentId: number;
  establishmentName: string;
}
export interface AppointmentSelectModel extends AppointmentBaseModel {
  id: number;
  active: boolean;
}

export interface AppointmentCreateModel extends AppointmentBaseModel { }


export interface AppointmentUpdateModel extends AppointmentBaseModel {
  id: number;
  active: boolean;
}
