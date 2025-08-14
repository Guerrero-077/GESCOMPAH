export interface DepartmentSelectModel {
  id: number;
  name: string;
  active: boolean;
}

export interface DepartmentCreate {
  name: string;
}

export interface DepartmentUpdate {
  id: number;
  name: string;
  active: boolean;
}
