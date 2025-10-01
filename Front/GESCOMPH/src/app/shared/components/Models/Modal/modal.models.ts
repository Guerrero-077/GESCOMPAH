import { Type } from '@angular/core';
import { FormType, FormSchema } from '../Form/form.models';

export interface GenericFormDialogData<T = any> {
    title: string;
    subtitle?: string;
    component: Type<any>;              // Componente dinámico como DynamicFormComponent
    schema: FormSchema;                // Schema completo con create, edit, view
    formType: FormType;                // 'create' | 'edit' | 'view'
    model?: Partial<T>;                // Datos del modelo (ej: al editar o ver)
    selectOptions?: Record<string, any[]>; // Opciones para campos tipo select o radio
    allowImageUpload?: boolean;        // Soporte opcional para subir imágenes
}
