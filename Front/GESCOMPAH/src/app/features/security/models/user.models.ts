// Corresponds to UserSelectDto from backend
export interface UserListDto {
  id: number; 
  personName: string;
  email: string;
  active: boolean;
}

// Corresponds to the full User object for viewing/editing details
export interface User {
  id: number;
  email: string;
  person: Person;
  rolUsers: RolUser[];
  isDeleted: boolean;
}

// Corresponds to UserCreateDto from backend
export interface UserCreateDto {
  email: string;
  password?: string; 
  personId: number;
  roleIds: number[]; 
}

// Corresponds to UserUpdateDto from backend
export interface UserUpdateDto {
  id: number;
  email: string;
  password?: string;
  personId: number;
  roleIds: number[];
}


// --- Person Models ---
export interface Person {
  id: number; 
  firstName: string;
  lastName: string;
  document?: string;
  address?: string;
  phone?: string;
  cityId: number;
}


// --- Role & RolUser Models ---
export interface Rol {
  id: number;
  name: string;
  description?: string;
  active: boolean;
}

export interface RolUser {
  userId: number;
  rolId: number;
  rol: Rol;
}
