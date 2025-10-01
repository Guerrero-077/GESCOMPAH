// features/security/models/person.models.ts
export interface PersonSelectModel {
  id: number;
  firstName: string;
  lastName: string;
  document?: string;
  address?: string;
  phone?: string;
  cityName: string;   // viene desde SelectDto
  email?: string;
}

export interface PersonCreateModel {
  firstName: string;
  lastName: string;
  document?: string;
  address?: string;
  phone?: string;
  cityId: number;
}

export interface PersonUpdateModel extends PersonCreateModel { }
