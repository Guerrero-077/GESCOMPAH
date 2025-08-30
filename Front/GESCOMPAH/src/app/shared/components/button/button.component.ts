import { Component, EventEmitter, Output } from '@angular/core';
import { HasRoleAndPermissionDirective } from '../../../core/Directives/HasRoleAndPermission.directive';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-button',
  imports: [HasRoleAndPermissionDirective],
  templateUrl: './button.component.html',
  styleUrl: './button.component.css'
})
export class ButtonComponent {
  @Output() onView = new EventEmitter<void>();
  @Output() onEdit = new EventEmitter<void>();
  @Output() onDelete = new EventEmitter<void>();

  handleView(): void {
    this.onView.emit();
  }

  handleEdit(): void {
    this.onEdit.emit();
  }

  handleDelete(): void {
    this.onDelete.emit();
  }
}
