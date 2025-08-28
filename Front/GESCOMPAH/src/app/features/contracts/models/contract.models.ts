export interface ContractCreateModel {
  startDate: string;
  endDate: string;
  address: string;
  cityId: number;
  document: string;
  firstName?: string;
  lastName?: string;
  phone?: string;
  email?: string;
  establishmentIds: number[];
  calculationKey: string;
}

export interface PremisesLeasedModel {
  id: number;
  establishmentId: number;
  establishmentName: string;
  plazaName?: string;
  address?: string;
}

export interface ContractSelectModel {
  id: number;
  startDate: string;
  endDate: string;
  status: string;
  personId: number;
  fullName: string;
  document: string;
  phone: string;
  email: string | null;
  premisesLeased: PremisesLeasedModel[];
}
