import { TemplateRef } from "@angular/core";

export interface TableColumn<T> {
    key: keyof T | string;
    header: string;
    sortable?: boolean;
    type?: 'text' | 'number' | 'boolean' | 'badge' | 'actions' | 'custom' | 'index';
    template?: TemplateRef<any>;
}
