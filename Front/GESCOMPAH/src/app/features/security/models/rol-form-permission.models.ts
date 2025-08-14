export interface RolFormPermissionSelectModel {
  id: number;
  rolName: string;
  formName: string;
  permissionName: string;
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
