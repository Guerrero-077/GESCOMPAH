export interface SquareCreateModel {
  name: string;
  description: string;
  location: string;
}
export interface SquareUpdateModel extends SquareCreateModel {
  id: number;
}
export interface SquareSelectModel extends SquareUpdateModel {
  active: boolean;
}
