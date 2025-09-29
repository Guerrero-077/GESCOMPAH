import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, TemplateRef } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { TableColumn } from '../../models/TableColumn.models';
import { HasRoleAndPermissionDirective } from '../../../core/security/directives/HasRoleAndPermission.directive';

export type FieldContext<T> = { $implicit: T; row: T };

@Component({
  selector: 'app-generic-card',
  standalone: true,
  imports: [CommonModule, MatIconModule, MatButtonModule, HasRoleAndPermissionDirective],
  templateUrl: './generic-card.component.html',
  styleUrls: ['./generic-card.component.css']
})
export class GenericCardComponent<T extends Record<string, any>> {
  @Input() data!: T;
  @Input() titleKey?: keyof T | string;
  @Input() subtitleKey?: keyof T | string;
  @Input() imageUrlKey?: keyof T | string;

  @Input() fields: TableColumn<T>[] = [];

  // Acciones
  @Input() showViewButton = true;
  @Input() showEditButton = true;
  @Input() showDeleteButton = true;

  @Output() view = new EventEmitter<T>();
  @Output() edit = new EventEmitter<T>();
  @Output() delete = new EventEmitter<T>();

  // Plantilla opcional para acciones personalizadas
  @Input() actionsTemplate?: TemplateRef<FieldContext<T>>;

  get img(): string | null {
    const key = this.imageUrlKey as string;
    if (!key) return null;
    const v = this.data?.[key];
    return typeof v === 'string' && v.length > 0 ? v : null;
  }

  get title(): string {
    const key = this.titleKey as string;
    const v = key ? this.data?.[key] : undefined;
    return (v ?? '').toString();
  }

  get subtitle(): string {
    const key = this.subtitleKey as string;
    const v = key ? this.data?.[key] : undefined;
    return (v ?? '').toString();
  }

  valueFor(col: TableColumn<T>): unknown {
    const key = col.key as string;
    if (col.render) return col.render(this.data);
    if (col.format) return col.format(this.data?.[key], this.data);
    return this.data?.[key];
  }
}

