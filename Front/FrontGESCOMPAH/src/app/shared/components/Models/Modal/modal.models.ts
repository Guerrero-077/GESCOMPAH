import { DynamicFormConfig } from "../Form/form.models";

export interface GenericFormDialogData {
    title: string;
    subtitle?: string;
    formConfig: DynamicFormConfig;
    model?: any;
    allowImageUpload?: boolean;
}
