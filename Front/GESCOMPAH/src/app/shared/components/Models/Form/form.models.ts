import { Type } from '@angular/core';

export type FormType = 'create' | 'edit' | 'view';

export type FormFieldType =
  | 'text'
  | 'number'
  | 'select'
  | 'textarea'
  | 'checkbox'
  | 'radio'
  | 'date'
  | 'email'
  | 'password'
  | 'hidden'
  | 'checkbox-list';

export interface FormFieldOption {
  label: string;
  value: any;
}

export interface DynamicFormField {
  name: string;
  /** Puede faltar o venir vacío => usamos fallback en el template */
  label?: string | null;
  type: FormFieldType;
  placeholder?: string;
  required?: boolean;
  disabled?: boolean;
  options?: FormFieldOption[];
  multiple?: boolean;
  /** Si es true y type === 'number', se mostrará con separador de miles (pesos) */
  currency?: boolean;
  validations?: {
    // texto
    minLength?: number;
    maxLength?: number;
    pattern?: string;
    onlySpaces?: boolean;
    email?: boolean;
    atLeastOne?: boolean;

    // numéricos
    min?: number;
    max?: number;
  };
}

export type FormSchema = Record<FormType, DynamicFormField[]>;
export type EntityFormSchemas = Record<string, FormSchema>;

export interface FormDialogData<T = any> {
  title?: string;
  component: Type<any>;
  formType: FormType;
  entity?: Partial<T>;
  schema: FormSchema;
  /** catálogos para selects: clave = nombre del campo */
  selectOptions?: Record<string, any[]>;
}
