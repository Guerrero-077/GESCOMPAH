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
    { name: 'formId', type: 'select', options: [], required: true },
    { name: 'moduleId', type: 'select', options: [], required: true },
    // { name: 'active', type: 'checkbox', required: true }
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
    { name: 'id', type: 'hidden', required: false },
    { name: 'firstName', type: 'text', required: true },
    { name: 'lastName', type: 'text', required: true },
    { name: 'document', type: 'text', required: false },
    { name: 'address', type: 'text', required: false },
    { name: 'phone', type: 'text', required: true },
    { name: 'cityId', type: 'select', required: true }
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
    { name: 'id', type: 'hidden', required: false },
    { name: 'rolId', type: 'select', options: [], required: true },
    { name: 'formId', type: 'select', options: [], required: true },
    { name: 'permissionId', type: 'select', options: [], required: true },
    { name: 'active', type: 'checkbox', required: true }
  ],

  'RolUser': [
    { name: 'id', type: 'hidden', required: false },
    { name: 'userId', type: 'select', required: true },
    { name: 'rolId', type: 'select', required: true },
    { name: 'active', type: 'checkbox', required: true }
  ],

  'User': [
    { name: 'Id', type: 'hidden', required: false },
    { name: 'email', type: 'text', required: true },
    { name: 'password', type: 'text', required: true },
    { name: 'personId', type: 'select', required: true }
  ]
}
