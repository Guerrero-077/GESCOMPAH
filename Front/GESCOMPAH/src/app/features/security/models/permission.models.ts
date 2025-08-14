export interface PermissionSelectModel {
  id: number;
  name: string;
  description: string;
  active: boolean;
}
export interface PermissionCreateModel {
  name: string;
  description: string;
}
export interface PermissionUpdateModel {
  id: number;
  name: string;
  description: string;
  active: boolean;
}
