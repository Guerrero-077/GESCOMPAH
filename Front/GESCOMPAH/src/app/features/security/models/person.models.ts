// features/security/models/person.models.ts
export interface Person {
  id: number;
  firstName: string;
  lastName: string;
  document?: string;
  address?: string;
  phone?: string;
  cityName: string;   // viene desde SelectDto
}

export interface PersonCreate {
  firstName: string;
  lastName: string;
  document?: string;
  address?: string;
  phone?: string;
  cityId: number;
}

export interface PersonUpdate extends PersonCreate { }
