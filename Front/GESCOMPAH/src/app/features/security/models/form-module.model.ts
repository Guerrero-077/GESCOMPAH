export interface FormModule {
  id: number;
  formId: number;
  moduleId: number;
  active: boolean;
}

export interface FormModuleCreateModel {
  formId: number;
  moduleId: number;
}

export interface FormModuleUpdateModel {
  id: number;
  formId: number;
  moduleId: number;
  active?: boolean; // Assuming active can be updated
}

export interface FormModuleSelectModel {
  id: number;
  formId: number;
  moduleId: number;
  formName: string; // Used in table display
  moduleName: string; // Used in table display
  active: boolean;
}
