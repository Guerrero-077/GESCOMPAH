export interface DynamicFormField {
    name: string;
    type: 'text' | 'date' | 'hidden' | 'select' | 'textarea' | 'checkbox';
    required: boolean;
    options?: { value: string | number; label: string }[];
}



export type FormType = 'Form' | 'Module' | 'FormModule' | 'Appointment' | 'City' | 'Department' |
    'Person' | 'Permission' | 'Rol' | 'RolFormPermission' | 'RolUser' | 'User';

export const formSchemas: Record<FormType, DynamicFormField[]> = {
    'Form': [
        {name: 'id', type: 'hidden', required: false},
        {name: 'name', type: 'text', required: true},
        {name: 'description', type: 'textarea', required: true},
        {name: 'route', type: 'text', required: true},
        {name: 'active', type: 'checkbox' , required: true}
        
    ],
    'Module': [
        {name: 'id', type: 'hidden', required: false},
        {name: 'name', type: 'text', required: true},
        {name: 'description', type: 'textarea', required: false},
        {name: 'icon', type: 'text', required: true},
        {name: 'active', type: 'checkbox', required: true}
    ],
    'FormModule': [
        {name: 'id', type: 'hidden', required: false},
        {name: 'form', type: 'select', options: [], required: true},
        {name: 'module', type: 'select', options: [], required: true},
        {name: 'active', type: 'checkbox', required: true}
    ],
    'Appointment': [
        {name: 'id', type: 'hidden', required: false},
        {name: 'fullName', type: 'text', required: true},
        {name: 'Email', type: 'text', required: true},
        {name: 'Phone', type: 'text', required: true},
        {name: 'Description', type: 'textarea', required: false},
        {name: 'RequestDate', type: 'date', required: true},
        {name: 'DateTimeAssigned', type: 'date', required: false},
        {name: 'Establishment', type: 'select', options: [], required: true},
    ],
    'City': [
        {name: 'Id', type: 'hidden', required: false},
        {name: 'Name', type: 'text', required: true},
        {name: 'Department', type: 'select', options: [], required: true},
    ],
    'Department': [
        {name: 'Id', type: 'hidden', required: false},
        {name: 'Name', type: 'text', required: true},
        {name: 'Active', type: 'checkbox', required: true}
    ],
    'Person': [
        {name: 'Id', type: 'hidden', required: false},
        {name: 'FirstName', type: 'text', required: true},
        {name: 'LastName', type: 'text', required: true},
        {name: 'Document', type: 'text', required: false},
        {name: 'Address', type: 'text', required: false},
        {name: 'Phone', type: 'text', required: true},
        {name: 'City', type: 'select', options: [], required: true},
        {name: 'User', type: 'select', options: [], required: false},
    ],
    'Permission': [
        {name: 'id', type: 'hidden', required: false},
        {name: 'name', type: 'text', required: true},
        {name: 'description', type: 'textarea', required: false},
        {name: 'active', type: 'checkbox', required: true}
    ],
    'Rol': [
        {name: 'id', type: 'hidden', required: false},
        {name: 'name', type: 'text', required: true},
        {name: 'description', type: 'textarea', required: false},
        {name: 'active', type: 'checkbox', required: true}
    ],
    'RolFormPermission': [
        {name: 'Id', type: 'hidden', required: false},
        {name: 'Rol', type: 'select', options: [], required: true},
        {name: 'FormModule', type: 'select', options: [], required: true},
        {name: 'Permission', type: 'select', options: [], required: true},
        {name: 'Active', type: 'checkbox', required: true}
    ],
    'RolUser': [
        {name: 'Id', type: 'hidden', required: false},
        {name: 'Rol', type: 'select', options: [], required: true},
        {name: 'User', type: 'select', options: [], required: true},
        {name: 'Active', type: 'checkbox', required: true}
    ],
    'User': [
        {name: 'Id', type: 'hidden', required: false},
        {name: 'Email', type: 'text', required: true},
        {name: 'Person', type: 'select', options: [], required: true},
        {name: 'Active', type: 'checkbox', required: true}
    ],
}