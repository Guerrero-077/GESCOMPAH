export interface DynamicFormField {
  name: string;
  label?: string;
  type: 'text' | 'date' | 'hidden' | 'select' | 'textarea' | 'checkbox' | 'checkbox-list';
  required: boolean;
  options?: { value: string | number; label: string }[]; // select y checkbox-list
  multiple?: boolean; // solo select
}

export type FormType =
  | 'Form' | 'Module' | 'FormModule' | 'Appointment' | 'City' | 'Department'
  | 'Person' | 'Permission' | 'Rol' | 'RolFormPermission' | 'RolUser' | 'User';

export const formSchemas: Record<FormType, DynamicFormField[]> = {
  'Form': [
    { name: 'id', type: 'hidden', required: false, label: 'Id' },
    { name: 'name', type: 'text', required: true, label: 'Nombre' },
    { name: 'description', type: 'textarea', required: true, label: 'Descripción' },
    { name: 'route', type: 'text', required: true, label: 'Ruta' },
  ],
  'Module': [
    { name: 'id', type: 'hidden', required: false, label: 'Id' },
    { name: 'name', type: 'text', required: true, label: 'Nombre' },
    { name: 'description', type: 'textarea', required: false, label: 'Descripción' },
    { name: 'icon', type: 'text', required: true, label: 'Ícono' },
  ],
  'FormModule': [
    { name: 'id', type: 'hidden', required: false, label: 'Id' },
    { name: 'formId', type: 'select', options: [], required: true, label: 'Formulario' },
    { name: 'moduleId', type: 'select', options: [], required: true, label: 'Módulo' },
  ],
  'Appointment': [
    { name: 'id', type: 'hidden', required: false, label: 'Id' },
    { name: 'fullName', type: 'text', required: true, label: 'Nombre completo' },
    { name: 'Email', type: 'text', required: true, label: 'Email' },
    { name: 'Phone', type: 'text', required: true, label: 'Teléfono' },
    { name: 'Description', type: 'textarea', required: false, label: 'Descripción' },
    { name: 'RequestDate', type: 'date', required: true, label: 'Fecha solicitud' },
    { name: 'DateTimeAssigned', type: 'date', required: false, label: 'Fecha asignada' },
    { name: 'Establishment', type: 'select', options: [], required: true, label: 'Establecimiento' },
  ],
  'City': [
    { name: 'id', type: 'hidden', required: false, label: 'Id' },
    { name: 'name', type: 'text', required: true, label: 'Ciudad' },
    { name: 'departmentId', type: 'select', required: true, label: 'Departamento' },
  ],
  'Department': [
    { name: 'id', type: 'hidden', required: false, label: 'Id' },
    { name: 'name', type: 'text', required: true, label: 'Departamento' },
  ],
  'Person': [
    { name: 'id', type: 'hidden', required: false, label: 'Id' },
    { name: 'firstName', type: 'text', required: true, label: 'Nombres' },
    { name: 'lastName', type: 'text', required: true, label: 'Apellidos' },
    { name: 'document', type: 'text', required: false, label: 'Documento' },
    { name: 'address', type: 'text', required: false, label: 'Dirección' },
    { name: 'phone', type: 'text', required: true, label: 'Teléfono' },
    { name: 'cityId', type: 'select', required: true, label: 'Ciudad' },
  ],
  'Permission': [
    { name: 'id', type: 'hidden', required: false, label: 'Id' },
    { name: 'name', type: 'text', required: true, label: 'Permiso' },
    { name: 'description', type: 'textarea', required: false, label: 'Descripción' },
  ],
  'Rol': [
    { name: 'id', type: 'hidden', required: false, label: 'Id' },
    { name: 'name', type: 'text', required: true, label: 'Rol' },
    { name: 'description', type: 'textarea', required: false, label: 'Descripción' },
  ],
  'RolFormPermission': [
    { name: 'id', type: 'hidden', required: false, label: 'Id' },
    { name: 'rolId', type: 'select', options: [], required: true, label: 'Rol' },
    { name: 'formId', type: 'select', options: [], required: true, label: 'Formulario' },
    // ← aquí está la magia: lista de checkboxes por ID
    { name: 'permissionIds', type: 'checkbox-list', options: [], required: true, label: 'Permisos' },
  ],
  'RolUser': [
    { name: 'id', type: 'hidden', required: false, label: 'Id' },
    { name: 'userId', type: 'select', required: true, label: 'Usuario' },
    { name: 'rolId', type: 'select', required: true, label: 'Rol' },
  ],
  'User': [
    { name: 'Id', type: 'hidden', required: false, label: 'Id' },
    { name: 'email', type: 'text', required: true, label: 'Email' },
    { name: 'password', type: 'text', required: true, label: 'Contraseña' },
    { name: 'personId', type: 'select', required: true, label: 'Persona' },
  ]
};
