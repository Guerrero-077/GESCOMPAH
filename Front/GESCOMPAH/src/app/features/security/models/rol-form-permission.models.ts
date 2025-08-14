export interface RolFormPermissionSelectModel {
  id: number;
  rolName: string;
  rolId: number;
  formName: string;
  formId: number;
  permissionName: string;
  permissionId: number;
  active: boolean;
}
export interface RolFormPermissionCreateModel {
  rolId: number;
  formId: number;
  permissionId: number;
}


export interface RolFormPermissionUpdateModel {
  id: number;
  rolId: number;
  formId: number;
  permissionId: number;
}
