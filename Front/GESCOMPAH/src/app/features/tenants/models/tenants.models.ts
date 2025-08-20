import { RoleSelectModel } from "../../security/models/role.models";

export interface TenantsSelectModel {
  id: number;
  email: string;
  roles: string[];
  active: boolean;

  personName: string;
  personDocument: string;
  personPhone: number;
  personAddress: string;
}


export interface TenantsCreateModel {
  id: number;
  email: string;
  password: string

  personId: number;
  cityId: number;
  roleIds?: number[];
}

export interface TenantsUpdateModel {
  id: number;
  email: string;
  password?: string

  personId: number;
  cityId: number;
  roleIds?: number[];
}


export type TenantFormMode = 'create' | 'edit';

export interface TenantFormData {
  mode: TenantFormMode;
  tenant?: {
    id?: number;
    email?: string;
    roleIds?: number[];
    roleName?: string | null;
    active?: boolean;


    departmentId?: number | null; // solo en create
    cityId?: number | null;

    personId?: number;
    firstName?: string;
    lastName?: string;
    document?: string;
    phone?: string | number;
    address?: string;

  };
}
