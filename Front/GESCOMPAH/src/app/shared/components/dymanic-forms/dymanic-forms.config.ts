import { DynamicFormField } from "../Models/Form/form.models";

export type FormType =
  | 'Form' | 'Module' | 'FormModule' | 'Appointment' | 'City' | 'Department'
  | 'Person' | 'Permission' | 'Rol' | 'RolFormPermission' | 'RolUser' | 'User' | 'Plaza' | 'Finance';

export const formSchemas: Record<FormType, DynamicFormField[]> = {
  'Form': [
    { name: 'id', type: 'hidden', required: false, label: 'Id' },
    {
      name: 'name', type: 'text', required: true, label: 'Nombre',
      validations: { minLength: 2, maxLength: 100, onlySpaces: true }
    },
    {
      name: 'description', type: 'textarea', required: true, label: 'Descripción',
      validations: { maxLength: 500, onlySpaces: true }
    },
    {
      name: 'route', type: 'text', required: true, label: 'Ruta',
      validations: { maxLength: 150, onlySpaces: true, pattern: '^/[a-z0-9/\\-_]*$' }
    },
  ],

  'Module': [
    { name: 'id', type: 'hidden', required: false, label: 'Id' },
    {
      name: 'name', type: 'text', required: true, label: 'Nombre',
      validations: { minLength: 2, maxLength: 100, onlySpaces: true }
    },
    {
      name: 'description', type: 'textarea', required: false, label: 'Descripción',
      validations: { maxLength: 500, onlySpaces: true }
    },
    {
      name: 'icon', type: 'text', required: true, label: 'Ícono',
      validations: { maxLength: 64, onlySpaces: true, pattern: '^[a-z_][a-z0-9_]*$' }
    },
  ],

  'FormModule': [
    { name: 'id', type: 'hidden', required: false, label: 'Id' },
    { name: 'formId', type: 'select', options: [], required: true, label: 'Formulario' },
    { name: 'moduleId', type: 'select', options: [], required: true, label: 'Módulo' },
  ],

  'Appointment': [
    { name: 'id', type: 'hidden', required: false, label: 'Id' },
    {
      name: 'fullName', type: 'text', required: true, label: 'Nombre completo',
      validations: { minLength: 3, maxLength: 150, onlySpaces: true }
    },
    {
      name: 'Email', type: 'email', required: true, label: 'Email',
      validations: { maxLength: 254, onlySpaces: true }
    },
    {
      name: 'Phone', type: 'text', required: true, label: 'Teléfono',
      validations: { minLength: 7, maxLength: 20, pattern: '^[0-9()+\\-\\s]{7,20}$' }
    },
    {
      name: 'Description', type: 'textarea', required: false, label: 'Descripción',
      validations: { maxLength: 500, onlySpaces: true }
    },
    { name: 'RequestDate', type: 'date', required: true, label: 'Fecha solicitud' },
    { name: 'DateTimeAssigned', type: 'date', required: false, label: 'Fecha asignada' },
    { name: 'Establishment', type: 'select', options: [], required: true, label: 'Establecimiento' },
  ],

  'City': [
    { name: 'id', type: 'hidden', required: false, label: 'Id' },
    {
      name: 'name', type: 'text', required: true, label: 'Ciudad',
      validations: { minLength: 2, maxLength: 100, onlySpaces: true, pattern: '^[\\p{L}\\s\\-\\.]+$' }
    },
    { name: 'departmentId', type: 'select', required: true, label: 'Departamento' },
  ],

  'Department': [
    { name: 'id', type: 'hidden', required: false, label: 'Id' },
    {
      name: 'name', type: 'text', required: true, label: 'Departamento',
      validations: { minLength: 2, maxLength: 100, onlySpaces: true, pattern: '^[\\p{L}\\s\\-\\.]+$' }
    },
  ],

  'Person': [
    { name: 'id', type: 'hidden', required: false, label: 'Id' },
    {
      name: 'firstName', type: 'text', required: true, label: 'Nombres',
      validations: { minLength: 2, maxLength: 100, onlySpaces: true, pattern: '^[\\p{L}\\s\\-\\.]+$' }
    },
    {
      name: 'lastName', type: 'text', required: true, label: 'Apellidos',
      validations: { minLength: 2, maxLength: 100, onlySpaces: true, pattern: '^[\\p{L}\\s\\-\\.]+$' }
    },
    {
      name: 'document', type: 'text', required: false, label: 'Documento',
      validations: { pattern: '^[0-9]{5,20}$' }
    },
    {
      name: 'address', type: 'text', required: false, label: 'Dirección',
      validations: { maxLength: 150, pattern: '^[\\p{L}\\d\\s#\\-,\\.]+$' }
    },
    {
      name: 'phone', type: 'text', required: true, label: 'Teléfono',
      validations: { minLength: 7, maxLength: 20, pattern: '^[0-9()+\\-\\s]{7,20}$' }
    },
    { name: 'cityId', type: 'select', required: true, label: 'Ciudad' },
  ],

  'Permission': [
    { name: 'id', type: 'hidden', required: false, label: 'Id' },
    {
      name: 'name', type: 'text', required: true, label: 'Permiso',
      validations: { minLength: 2, maxLength: 100, onlySpaces: true }
    },
    {
      name: 'description', type: 'textarea', required: false, label: 'Descripción',
      validations: { maxLength: 500, onlySpaces: true }
    },
  ],

  'Rol': [
    { name: 'id', type: 'hidden', required: false, label: 'Id' },
    {
      name: 'name', type: 'text', required: true, label: 'Rol',
      validations: { minLength: 2, maxLength: 100, onlySpaces: true }
    },
    {
      name: 'description', type: 'textarea', required: false, label: 'Descripción',
      validations: { maxLength: 500, onlySpaces: true }
    },
  ],

  'RolFormPermission': [
    { name: 'id', type: 'hidden', required: false, label: 'Id' },
    { name: 'rolId', type: 'select', options: [], required: true, label: 'Rol' },
    { name: 'formId', type: 'select', options: [], required: true, label: 'Formulario' },
    {
      name: 'permissionIds', type: 'checkbox-list', options: [], required: true, label: 'Permisos',
      validations: { atLeastOne: true }
    },
  ],

  'RolUser': [
    { name: 'id', type: 'hidden', required: false, label: 'Id' },
    { name: 'userId', type: 'select', required: true, label: 'Usuario' },
    { name: 'rolId', type: 'select', required: true, label: 'Rol' },
  ],

  'User': [
    { name: 'Id', type: 'hidden', required: false, label: 'Id' },
    {
      name: 'email', type: 'email', required: true, label: 'Email',
      validations: { maxLength: 254, onlySpaces: true }
    },
    {
      name: 'password', type: 'password', required: true, label: 'Contraseña',
      validations: { minLength: 6, maxLength: 128, onlySpaces: true }
    },
    { name: 'personId', type: 'select', required: true, label: 'Persona' },
  ],

  'Plaza': [
    { name: 'id', type: 'hidden', label: 'Id', required: false },
    {
      name: 'name', label: 'Nombre', type: 'text', required: true,
      validations: { maxLength: 100, onlySpaces: true }
    },
    {
      name: 'description', label: 'Descripción', type: 'textarea', required: true,
      validations: { maxLength: 500, onlySpaces: true }
    },
    {
      name: 'location', label: 'Ubicación', type: 'text', required: true,
      validations: { maxLength: 150, onlySpaces: true, pattern: "^[\\p{L}\\d\\s#\\-,\\.]+$" }
    },
  ],

  // dymanic-forms.config.ts (solo el bloque Finance)

  'Finance': [
    { name: 'id', type: 'hidden', required: false, label: 'Id' },
    {
      name: 'key', type: 'text', required: true, label: 'Nombre',
      validations: {
        minLength: 3, maxLength: 64, onlySpaces: true,
        pattern: '^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ0-9_]{3,64}$'
      }
    },
    {
      name: 'value', type: 'number', required: true, label: 'Valor',
      validations: {
        min: 0,
        max: 1000000,
        pattern: '^(?:\\d+|\\d+[\\.,]\\d{1,2})$'
      }
    },
    { name: 'effectiveFrom', type: 'date', required: true, label: 'Vigente desde' },
    { name: 'effectiveTo', type: 'date', required: true, label: 'Vigente hasta' },
  ],


};
