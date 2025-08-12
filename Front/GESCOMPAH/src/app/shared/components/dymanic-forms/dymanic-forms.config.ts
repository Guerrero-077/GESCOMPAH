import { Observable } from "rxjs";

export interface DynamicFormField {
  name: string;
  type: 'text' | 'date' | 'hidden' | 'select' | 'textarea' | 'checkbox';
  required: boolean;
  options?: { value: string | number; label: string }[];
  multiple?: boolean;
  // loadOptions?: () => Observable<{ value: string | number; label: string }[]>; // Removed from here
}



export type FormType = 'Form' | 'Module' | 'FormModule' | 'Appointment' | 'City' | 'Department' |
  'Person' | 'Permission' | 'Rol' | 'RolFormPermission' | 'RolUser' | 'User';

export const formSchemas: Record<FormType, DynamicFormField[]> = {
  'Form': [
    { name: 'id', type: 'hidden', required: false },
    { name: 'name', type: 'text', required: true },
    { name: 'description', type: 'textarea', required: true },
    { name: 'route', type: 'text', required: true },
    // {name: 'active', type: 'checkbox' , required: true}

  ],
  'Module': [
    { name: 'id', type: 'hidden', required: false },
    { name: 'name', type: 'text', required: true },
    { name: 'description', type: 'textarea', required: false },
    { name: 'icon', type: 'text', required: true },
    // {name: 'active', type: 'checkbox', required: true}
  ],
  'FormModule': [
    { name: 'id', type: 'hidden', required: false },
    { name: 'form', type: 'select', options: [], required: true },
    { name: 'module', type: 'select', options: [], required: true },
    //{name: 'active', type: 'checkbox', required: true}
  ],
  'Appointment': [
    { name: 'id', type: 'hidden', required: false },
    { name: 'fullName', type: 'text', required: true },
    { name: 'Email', type: 'text', required: true },
    { name: 'Phone', type: 'text', required: true },
    { name: 'Description', type: 'textarea', required: false },
    { name: 'RequestDate', type: 'date', required: true },
    { name: 'DateTimeAssigned', type: 'date', required: false },
    { name: 'Establishment', type: 'select', options: [], required: true },
  ],
  'City': [
    { name: 'id', type: 'hidden', required: false },
    { name: 'name', type: 'text', required: true },
    { name: 'departmentId', type: 'select', required: true }, // Removed loadOptions
    { name: 'active', type: 'checkbox', required: true }
  ],
  'Department': [
    { name: 'id', type: 'hidden', required: false },
    { name: 'name', type: 'text', required: true },
    { name: 'active', type: 'checkbox', required: true }
  ],
  'Person': [
    { name: 'Id', type: 'hidden', required: false },
    { name: 'FirstName', type: 'text', required: true },
    { name: 'LastName', type: 'text', required: true },
    { name: 'Document', type: 'text', required: false },
    { name: 'Address', type: 'text', required: false },
    { name: 'Phone', type: 'text', required: true },
    { name: 'Department', type: 'select', options: [], required: true },
    { name: 'City', type: 'select', options: [], required: true }
  ],

  'Permission': [
    { name: 'id', type: 'hidden', required: false },
    { name: 'name', type: 'text', required: true },
    { name: 'description', type: 'textarea', required: false },
    //{name: 'active', type: 'checkbox', required: true}
  ],
  'Rol': [
    { name: 'id', type: 'hidden', required: false },
    { name: 'name', type: 'text', required: true },
    { name: 'description', type: 'textarea', required: false },
    //{name: 'active', type: 'checkbox', required: true}
  ],
  'RolFormPermission': [
    { name: 'Id', type: 'hidden', required: false },
    { name: 'Rol', type: 'select', options: [], required: true },
    { name: 'FormModule', type: 'select', options: [], required: true },
    { name: 'Permission', type: 'select', options: [], required: true },
    //{name: 'active', type: 'checkbox', required: true}
  ],
  'RolUser': [
    { name: 'Id', type: 'hidden', required: false },
    { name: 'Rol', type: 'select', options: [], required: true },
    { name: 'User', type: 'select', options: [], required: true },
    //{name: 'active', type: 'checkbox', required: true}
  ],
  'User': [
    { name: 'Id', type: 'hidden', required: false },
    { name: 'Email', type: 'text', required: true },
    { name: 'Password', type: 'text', required: true },
    { name: 'personId', type: 'select', required: true },
    { name: 'roleIds', type: 'select', required: true, multiple: true },
  ],

}
