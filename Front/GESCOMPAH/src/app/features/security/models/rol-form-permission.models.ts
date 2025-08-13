export interface RolFormPermissionCreateDto {
  rolId: number;
  formId: number;
  permissionId: number;
}

export interface RolFormPermissionSelectDto {
  id: number;
  rolName: string;
  formName: string;
  permissionName: string;
  active: boolean;
}

export interface RolFormPermissionUpdateDto {
  id: number;
  rolId: number;
  formId: number;
  permissionId: number;
}
