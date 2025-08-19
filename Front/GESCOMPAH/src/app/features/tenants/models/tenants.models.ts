import { RoleSelectModel } from "../../security/models/role.models";

export interface TenantsSelectModel {
  personName: string;
  personDocument: string;
  personPhone: number;
  personAddress: string;
  email: string;
  roles: string[];
  id: number;
  active: boolean;
}
export interface TenantsCreateModel {
  email: string;
}
export interface TenantsUpdateModel {
  personId: number;
  email: string;
  active: boolean;
}
