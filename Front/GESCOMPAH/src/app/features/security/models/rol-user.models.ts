export interface RolUserSelectModel {
  id: number;
  userId: number;
  userEmail: string;
  rolId: number;
  rolName: string;
  active: boolean;
}

export interface RolUserCreateModel {
  userId: number;
  rolId: number;
  active: boolean;
}

export interface RolUserUpdateModel {
  userId: number;
  rolId: number;
  active: boolean;
}
