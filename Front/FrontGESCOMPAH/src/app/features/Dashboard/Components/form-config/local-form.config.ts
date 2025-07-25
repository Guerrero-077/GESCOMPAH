import { LocalesModel } from "../../../../shared/components/Models/card/card.models";
import { DynamicFormConfig } from "../../../../shared/components/Models/Form/form.models";

export function buildLocalFormConfig(local: LocalesModel): DynamicFormConfig {
    return {
        submitButtonLabel: 'Guardar',
        fields: [
            {
                name: 'name',
                label: 'Nombre',
                type: 'text',
                value: local?.name || '',
                validations: { required: true },
                placeholder: 'Ej: Local 101'
            },
            {
                name: 'description',
                label: 'Descripción',
                type: 'textarea',
                value: local?.description || '',
                placeholder: 'Descripción del local'
            },
            {
                name: 'areaM2',
                label: 'Área (m²)',
                type: 'number',
                value: local?.areaM2 || '',
                placeholder: 'Ej: 25'
            },
            {
                name: 'rentValueBase',
                label: 'Valor base de arriendo',
                type: 'number',
                value: local?.rentValueBase || '',
                placeholder: 'Ej: 300000'
            }
        ]
    };
}
