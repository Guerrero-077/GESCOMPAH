import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';


import {
  IonButton,
  IonIcon
} from '@ionic/angular/standalone';


@Component({
  selector: 'app-button',
  standalone: true,
  imports: [CommonModule, IonButton, IonIcon],
  templateUrl: './button.component.html',
  styleUrl: './button.component.scss'
})
export class ButtonComponent {
  @Input() showView = true;
  @Input() showEdit = true;
  @Input() showDelete = true;

  @Output() onView = new EventEmitter<void>();
  @Output() onEdit = new EventEmitter<void>();
  @Output() onDelete = new EventEmitter<void>();

  handleView() {
    this.onView.emit();
  }

  handleEdit() {
    this.onEdit.emit();
  }

  handleDelete() {
    this.onDelete.emit();
  }
}
