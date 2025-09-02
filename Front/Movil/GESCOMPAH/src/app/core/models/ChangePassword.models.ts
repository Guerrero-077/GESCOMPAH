export interface ChangePasswordDto {
  userId: number;
  currentPassword: string;
  newPassword: string;
}
