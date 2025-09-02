
export interface UserSelectModel {
  id: number;
  personName: string;
  email: string;
  active: boolean;
}

// Crear
export interface UserCreateModel {
  email: string;
  password: string;
  personId: number;
}

// Actualizar (sin id en el body)
export interface UserUpdateModel {
  email: string;
  password?: string;
}

