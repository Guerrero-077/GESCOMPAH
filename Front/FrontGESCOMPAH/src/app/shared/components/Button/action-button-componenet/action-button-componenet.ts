import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-action-button-componenet',
  imports: [CommonModule],
  templateUrl: './action-button-componenet.html',
  styleUrl: './action-button-componenet.css'
})
export class ActionButtonComponenet {
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