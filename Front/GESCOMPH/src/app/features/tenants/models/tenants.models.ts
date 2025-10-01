export interface TenantFormData {
  mode: 'create' | 'edit';
  tenant?: TenantsSelectModel;
}

export interface TenantsSelectModel {
  id: number;

  personId: number;
  /** 👇 NUEVO: backend entrega separados */
  personFirstName: string;
  personLastName: string;

  /** (opcional) conservar por compatibilidad/mostrar */
  personName: string;

  personDocument: string;
  personAddress: string;
  personPhone: string;

  email: string;

  cityId: number;
  cityName: string;

  active: boolean;
  roles: string[];
}

export interface TenantsCreateModel {
  firstName: string;
  lastName: string;
  document: string;
  phone: string;
  address: string;
  cityId: number;
  email: string;
  roleIds: number[];
}

export interface TenantsUpdateModel {
  id: number;
  firstName: string;
  lastName: string;
  phone: string;
  address: string;
  cityId: number;
  email: string;
  roleIds: number[];
  active?: boolean;
}
