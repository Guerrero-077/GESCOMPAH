// shared/components/dynamic-form/dynamic-form.types.ts

export type FormFieldType =
    | 'text'
    | 'number'
    | 'select'
    | 'textarea'
    | 'checkbox'
    | 'radio'
    | 'date'
    | 'email'
    | 'password';

export interface DynamicFormField {
    name: string;
    label: string;
    type: FormFieldType;
    value?: any;
    placeholder?: string;
    options?: { label: string; value: any }[]; // para select y radio
    validations?: {
        required?: boolean;
        minLength?: number;
        maxLength?: number;
        pattern?: string;
    };
}

export interface DynamicFormConfig {
    fields: DynamicFormField[];
    submitButtonLabel?: string;
}
