export interface RolUserListDto {
  id: number;
  userId: number;
  userEmail: string;
  rolId: number;
  rolName: string;
  active: boolean;
}

export interface RolUserCreateDto {
  userId: number;
  rolId: number;
  active: boolean;
}

export interface RolUserUpdatePayload {
  userId: number;
  rolId: number;
  active: boolean;
}
