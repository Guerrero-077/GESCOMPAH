export interface FormModuleSelectModel {
  id: number;            // viene de BaseDto
  formName: string;
  moduleName: string;
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
}
