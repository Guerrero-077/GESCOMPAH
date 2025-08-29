export interface ContractCreateModel {
  startDate: string;  // "YYYY-MM-DD"
  endDate:   string;  // "YYYY-MM-DD"
  address: string;
  cityId: number;
  document: string;
  firstName: string;
  lastName: string;
  phone: string;
  email?: string | null;
  establishmentIds: number[];
  useSystemParameters?: boolean;
  clauseIds?: number[];
}

/** Detalle de locales asociados (para lista completa) */
export interface PremisesLeasedModel {
  id: number;
  establishmentId: number;
  establishmentName: string;
  plazaName?: string;
  address?: string;
}

/** Read-model COMPLETO (Admin) */
export interface ContractSelectModel {
  id: number;
  startDate: string; // "YYYY-MM-DD"
  endDate:   string; // "YYYY-MM-DD"
  personId: number;
  fullName: string;
  document: string;
  phone: string;
  email: string | null;
  premisesLeased: PremisesLeasedModel[];
  active: boolean;
}

/** Read-model LIGERO (/contract/mine) */
export interface ContractCard {
  id: number;
  personId: number;
  startDate: string;  // "YYYY-MM-DD"
  endDate:   string;  // "YYYY-MM-DD"
  totalBase: number;
  totalUvt:  number;
  active:    boolean;
}
