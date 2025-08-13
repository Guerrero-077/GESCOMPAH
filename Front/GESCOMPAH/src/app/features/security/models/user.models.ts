
export interface UserListDto {
  id: number;
  personName: string;
  email: string;
  active: boolean;
}

// Crear
export interface UserCreateDto {
  email: string;
  password: string;
  personId: number;
}

// Actualizar (sin id en el body)
export interface UserUpdatePayload {
  email: string;
  password?: string;
}

