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
  permissionIds: number[];
}


export interface RolFormPermissionUpdateModel {
  id: number;
  rolId: number;
  formId: number;
  permissionIds: number[];
  active: boolean;

}

// --- NUEVOS MODELOS PARA LA VISTA AGRUPADA ---
export interface RolFormPermissionGroupedModel {
  rolId: number;
  rolName: string;
  formId: number;
  formName: string;
  permissions: PermissionInfo[];
  active: boolean;
}

export interface PermissionInfo {
  permissionId: number;
  permissionName: string;
}


