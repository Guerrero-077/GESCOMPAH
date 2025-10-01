import { DynamicFormField } from "../Models/Form/form.models";

export type FormType =
| 'Form' | 'Module' | 'FormModule' | 'Appointment' | 'City' | 'Department'
| 'Person' | 'Permission' | 'Rol' | 'RolFormPermission' | 'RolUser' | 'User' | 'Plaza' | 'Finance';

export const formSchemas: Record<FormType, DynamicFormField[]> = {
'Form': [
  { name: 'id', type: 'hidden', required: false, label: 'Id' },
  {
    name: 'name', type: 'text', required: true, label: 'Nombre',
    placeholder: 'Formulario de Contratos',
    validations: { minLength: 2, maxLength: 100, onlySpaces: true }
  },
  {
    name: 'description', type: 'textarea', required: true, label: 'Descripción',
    placeholder: 'Formulario para gestionar los contratos de arrendamiento de locales comerciales en las plazas de mercado.',
    validations: { maxLength: 500, onlySpaces: true }
  },
  {
    name: 'route', type: 'text', required: true, label: 'Ruta',
    placeholder: '/contracts/list',
    validations: { maxLength: 150, onlySpaces: true, pattern: '^/[a-z0-9/\\-_]*$' }
  },
],

'Module': [
  { name: 'id', type: 'hidden', required: false, label: 'Id' },
  {
    name: 'name', type: 'text', required: true, label: 'Nombre',
    placeholder: 'Gestión Comercial',
    validations: { minLength: 2, maxLength: 100, onlySpaces: true }
  },
  {
    name: 'description', type: 'textarea', required: false, label: 'Descripción',
    placeholder: 'Módulo para la administración y control de actividades comerciales en las plazas de mercado.',
    validations: { maxLength: 500, onlySpaces: true }
  },
  {
    name: 'icon', type: 'text', required: true, label: 'Ícono',
    placeholder: 'business_center',
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
    placeholder: 'Carlos Alberto García Rodríguez',
    validations: { minLength: 3, maxLength: 150, onlySpaces: true }
  },
  {
    name: 'Email', type: 'email', required: true, label: 'Email',
    placeholder: 'carlos.garcia@gmail.com',
    validations: { maxLength: 254, onlySpaces: true }
  },
  {
    name: 'Phone', type: 'text', required: true, label: 'Teléfono',
    placeholder: '3012345678',
    validations: { minLength: 7, maxLength: 20, pattern: '^[0-9()+\\-\\s]{7,20}$' }
  },
  {
    name: 'Description', type: 'textarea', required: false, label: 'Descripción',
    placeholder: 'Solicito una cita para revisar las condiciones de arrendamiento del local 23 en la Plaza Central.',
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
    placeholder: 'Palermo',
    validations: { minLength: 2, maxLength: 100, onlySpaces: true, pattern: '^[\\p{L}\\s\\-\\.]+$' }
  },
  { name: 'departmentId', type: 'select', required: true, label: 'Departamento' },
],

'Department': [
  { name: 'id', type: 'hidden', required: false, label: 'Id' },
  {
    name: 'name', type: 'text', required: true, label: 'Departamento',
    placeholder: 'Huila',
    validations: { minLength: 2, maxLength: 100, onlySpaces: true, pattern: '^[\\p{L}\\s\\-\\.]+$' }
  },
],

'Person': [
  { name: 'id', type: 'hidden', required: false, label: 'Id' },
  {
    name: 'firstName', type: 'text', required: true, label: 'Nombres',
    placeholder: 'Carlos Alberto',
    validations: { minLength: 2, maxLength: 100, onlySpaces: true, pattern: '^[\\p{L}\\s\\-\\.]+$' }
  },
  {
    name: 'lastName', type: 'text', required: true, label: 'Apellidos',
    placeholder: 'García Rodríguez',
    validations: { minLength: 2, maxLength: 100, onlySpaces: true, pattern: '^[\\p{L}\\s\\-\\.]+$' }
  },
  {
    name: 'document', type: 'text', required: false, label: 'Documento',
    placeholder: '12345678',
    validations: { pattern: '^[0-9]{5,20}$' }
  },
  {
    name: 'address', type: 'text', required: false, label: 'Dirección',
    placeholder: 'Calle 26 # 47 - 15 Apto 302',
    validations: { maxLength: 150, pattern: '^[\\p{L}\\d\\s#\\-,\\.]+$' }
  },
  {
    name: 'phone', type: 'text', required: true, label: 'Teléfono',
    placeholder: '3012345678',
    validations: { minLength: 7, maxLength: 20, pattern: '^[0-9()+\\-\\s]{7,20}$' }
  },
  { name: 'cityId', type: 'select', required: true, label: 'Ciudad' },
],

'Permission': [
  { name: 'id', type: 'hidden', required: false, label: 'Id' },
  {
    name: 'name', type: 'text', required: true, label: 'Permiso',
    placeholder: 'Crear Contratos',
    validations: { minLength: 2, maxLength: 100, onlySpaces: true }
  },
  {
    name: 'description', type: 'textarea', required: false, label: 'Descripción',
    placeholder: 'Permite crear nuevos contratos de arrendamiento para los locales comerciales.',
    validations: { maxLength: 500, onlySpaces: true }
  },
],

'Rol': [
  { name: 'id', type: 'hidden', required: false, label: 'Id' },
  {
    name: 'name', type: 'text', required: true, label: 'Rol',
    placeholder: 'Administrador de Plaza',
    validations: { minLength: 2, maxLength: 100, onlySpaces: true }
  },
  {
    name: 'description', type: 'textarea', required: false, label: 'Descripción',
    placeholder: 'Rol encargado de administrar y supervisar las actividades comerciales de una plaza de mercado específica.',
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
    placeholder: 'carlos.garcia@gmail.com',
    validations: { maxLength: 254, onlySpaces: true }
  },
  {
    name: 'password', type: 'password', required: true, label: 'Contraseña',
    placeholder: '••••••••',
    validations: { minLength: 6, maxLength: 128, onlySpaces: true }
  },
  { name: 'personId', type: 'select', required: true, label: 'Persona' },
],

'Plaza': [
  { name: 'id', type: 'hidden', label: 'Id', required: false },
  {
    name: 'name', label: 'Nombre', type: 'text', required: true,
    placeholder: 'Plaza de Mercado Central',
    validations: { maxLength: 100, onlySpaces: true }
  },
  {
    name: 'description', label: 'Descripción', type: 'textarea', required: true,
    placeholder: 'Plaza de mercado ubicada en el centro de la ciudad, cuenta con 45 locales comerciales distribuidos en dos plantas, especializados en venta de alimentos, carnes, verduras y productos varios.',
    validations: { maxLength: 500, onlySpaces: true }
  },
  {
    name: 'location', label: 'Ubicación', type: 'text', required: true,
    placeholder: 'Carrera 5 # 12 - 30 Centro',
    validations: { maxLength: 150, onlySpaces: true, pattern: "^[\\p{L}\\d\\s#\\-,\\.]+$" }
  },
],

// dymanic-forms.config.ts (solo el bloque Finance)

'Finance': [
  { name: 'id', type: 'hidden', required: false, label: 'Id' },
  {
    name: 'key', type: 'text', required: true, label: 'Nombre',
    placeholder: 'VALOR_UVT_2024',
    validations: {
      minLength: 3, maxLength: 64, onlySpaces: true,
      pattern: '^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ0-9_]{3,64}$'
    }
  },
  {
    name: 'value', type: 'number', required: true, label: 'Valor',
    placeholder: '47065',
    currency: true,
    validations: {
      min: 0,
      // Acepta valores mayores (SMLV, etc.)
      max: 100000000000,
      // Permite miles con punto y decimales opcionales con coma o punto
      // Ej válidos: 1676666, 1.676.666, 1.234,56, 1234.56
      pattern: '^(?:\\d{1,3}(?:[\\.\\s]\\d{3})*|\\d+)(?:[\\.,]\\d{1,2})?$'
    }
  },
  { name: 'effectiveFrom', type: 'date', required: true, label: 'Vigente desde' },
  { name: 'effectiveTo', type: 'date', required: true, label: 'Vigente hasta' },
],


};
