import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-button',
  imports: [],
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