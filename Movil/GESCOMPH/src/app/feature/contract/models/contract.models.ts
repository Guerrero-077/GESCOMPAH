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

// Para listar (detalle de locales, solo cuando se usa el modelo completo)
export interface PremisesLeasedModel {
  id: number;
  establishmentId: number;
  establishmentName: string;
  plazaName?: string;
  address?: string;
}

/** Detalle (ver/editar) */
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

/** Read-model ÚNICO para la grilla (admin/arrendador; backend decide por rol) */
export interface ContractCard {
  id: number;
  personId: number;
  personFullName: string;
  personDocument: string;
  personPhone: string;
  personEmail: string | null;
  startDate: string;  // "YYYY-MM-DD"
  endDate:   string;  // "YYYY-MM-DD"
  totalBase: number;
  totalUvt:  number;
  active:    boolean;
}
