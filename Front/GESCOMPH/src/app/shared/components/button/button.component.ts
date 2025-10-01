import { Component, EventEmitter, Input, Output } from '@angular/core';
import { HasRoleAndPermissionDirective } from '../../../core/Directives/HasRoleAndPermission.directive';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-button',
  imports: [HasRoleAndPermissionDirective, CommonModule],
  templateUrl: './button.component.html',
  styleUrl: './button.component.css'
})
export class ButtonComponent {
  @Input() showView = true;
  @Input() showEdit = true;
  @Input() showDelete = true;


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
