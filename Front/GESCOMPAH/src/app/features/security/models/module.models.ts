export interface ModuleSelectModel {
  id: number;
  name: string;
  description: string;
  icon: string;
  active: boolean;
}
export interface ModuleCreateModel {
  name: string;
  description: string;
  icon: string;
}
export interface ModuleUpdateModel {
  id: number;
  name: string;
  description: string;
  icon: string;
  active: boolean;
}
