export interface AppointmentBaseModel {
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

export interface AppointmentSelect extends AppointmentBaseModel
{
  id: number;
  personId: number;
  personName: string;
  status: number;
  statusName: string;
  active: boolean;
}
