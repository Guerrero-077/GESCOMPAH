// form-module-create.model.ts
export interface FormModuleCreateDto {
  formId: number;
  moduleId: number;
}

// form-module-select.model.ts
export interface FormModuleSelectDto {
  id: number;            // viene de BaseDto
  formName: string;
  moduleName: string;
  active: boolean;
}

// form-module-update.model.ts
export interface FormModuleUpdateDto {
  id: number;            // viene de BaseDto
  formId: number;
  moduleId: number;
}
