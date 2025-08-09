

// export type FormFieldType =
//     | 'text'
//     | 'number'
//     | 'select'
//     | 'textarea'
//     | 'checkbox'
//     | 'radio'
//     | 'date'
//     | 'email'
//     | 'password'
//     | 'hidden';

import { Type } from "@angular/core";

// export interface DynamicFormField {
//     name: string;
//     label: string;
//     type: FormFieldType;    
//     value?: any;
//     placeholder?: string;
//     options?: { label: string; value: any }[]; // para select y radio
//     validations?: {
//         required?: boolean;
//         minLength?: number;
//         maxLength?: number;
//         pattern?: string;
//     };
// }

// export interface DynamicFormConfig {
//     fields: DynamicFormField[];
//     submitButtonLabel?: string;
// }

// form.types.ts

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
    | 'hidden';

export interface FormFieldOption {
    label: string;
    value: any;
}

export interface DynamicFormField {
    name: string;
    label: string;
    type: FormFieldType;
    placeholder?: string;
    required?: boolean;
    disabled?: boolean;
    options?: FormFieldOption[]; // solo para select o radio
    validations?: {
        minLength?: number;
        maxLength?: number;
        pattern?: string;
    };
}

export type FormSchema = Record<FormType, DynamicFormField[]>;
export type EntityFormSchemas = Record<string, FormSchema>;

export interface FormDialogData<T = any> {
    title?: string;
    component: Type<any>; // componente que será renderizado dinámicamente
    formType: FormType;
    entity?: Partial<T>;
    schema: FormSchema;
    selectOptions?: Record<string, any[]>;
}
