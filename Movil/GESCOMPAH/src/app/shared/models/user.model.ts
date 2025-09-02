import { BackendMenuItem } from "./sidebar.models";


export interface User {
  id: number;
  fullName: string ;
  email: string;
  roles: string[];
  menu: BackendMenuItem[];
}
