export type FormType = 'Form' | 'Module' | 'FormModule' | 'Appointment' | 'City' | 'Department' | 
'Person' | 'Permission' | 'Rol' | 'RolFormPermission' | 'RolUser' | 'User';

export const formSchemas: Record<FormType, any[]> = {
    'Form': [
        {name: 'Name', type: 'text', required: true},
        {name: 'Description', type: 'textarea', required: false},
        {name: 'Route', type: 'text', required: true},
        {name: 'Active', type: 'checkbox', required: true}
        
    ],
    'Module': [
        {name: 'Name', type: 'text', required: true},
        {name: 'Description', type: 'textarea', required: false},
        {name: 'Icon', type: 'text', required: true},
        {name: 'Active', type: 'checkbox', required: true}
    ],
    'FormModule': [
        {name: 'Form', type: 'select', options: [], required: true},
        {name: 'Module', type: 'select', options: [], required: true},
        {name: 'Active', type: 'checkbox', required: true}
    ],
    'Appointment': [
        {name: 'FullName', type: 'text', required: true},
        {name: 'Email', type: 'text', required: true},
        {name: 'Phone', type: 'text', required: true},
        {name: 'Description', type: 'textarea', required: false},
        {name: 'RequestDate', type: 'date', required: true},
        {name: 'DateTimeAssigned', type: 'date', required: false},
        {name: 'Establishment', type: 'select', options: [], required: true},
    ],
    'City': [
        {name: 'Name', type: 'text', required: true},
        {name: 'Department', type: 'select', options: [], required: true},
    ],
    'Department': [
        {name: 'Name', type: 'text', required: true},
        {name: 'Active', type: 'checkbox', required: true}
    ],
    'Person': [
        {name: 'FirstName', type: 'text', required: true},
        {name: 'LastName', type: 'text', required: true},
        {name: 'Document', type: 'text', required: false},
        {name: 'Address', type: 'text', required: false},
        {name: 'Phone', type: 'text', required: true},
        {name: 'City', type: 'select', options: [], required: true},
        {name: 'User', type: 'select', options: [], required: false},
    ],
    'Permission': [
        {name: 'Name', type: 'text', required: true},
        {name: 'Description', type: 'textarea', required: false},
        {name: 'Active', type: 'checkbox', required: true}
    ],
    'Rol': [
        {name: 'Name', type: 'text', required: true},
        {name: 'Description', type: 'textarea', required: false},
        {name: 'Active', type: 'checkbox', required: true}
    ],
    'RolFormPermission': [
        {name: 'Rol', type: 'select', options: [], required: true},
        {name: 'FormModule', type: 'select', options: [], required: true},
        {name: 'Permission', type: 'select', options: [], required: true},
        {name: 'Active', type: 'checkbox', required: true}
    ],
    'RolUser': [
        {name: 'Rol', type: 'select', options: [], required: true},
        {name: 'User', type: 'select', options: [], required: true},
        {name: 'Active', type: 'checkbox', required: true}
    ],
    'User': [
        {name: 'Email', type: 'text', required: true},
        {name: 'Person', type: 'select', options: [], required: true},
        {name: 'Active', type: 'checkbox', required: true}
    ],
}