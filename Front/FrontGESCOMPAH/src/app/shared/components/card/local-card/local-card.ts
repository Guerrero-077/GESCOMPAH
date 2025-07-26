import { Component, EventEmitter, Input, Output } from '@angular/core';
import { LocalesModel } from '../../Models/card/card.models';
import { ActionButtonComponenet } from "../../Button/action-button-componenet/action-button-componenet";
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-local-card',
  imports: [ActionButtonComponenet, CommonModule],
  templateUrl: './local-card.html',
  styleUrl: './local-card.css'
})
export class LocalCard {
  @Input() local!: LocalesModel;
  @Input() showAvailableTag: boolean = true;

  @Output() onView = new EventEmitter<number>();
  @Output() onEdit = new EventEmitter<number>();
  @Output() onDelete = new EventEmitter<number>();

  get primaryImage(): string {
    return this.local?.images?.[0]?.filePath || 'assets/images/placeholder.jpg';
  }

  get formattedRent(): string {
    return new Intl.NumberFormat('es-CO', {
      style: 'currency',
      currency: 'COP',
      minimumFractionDigits: 0
    }).format(this.local?.rentValueBase || 0);
  }

  handleView(): void {
    this.onView.emit(this.local.id);
  }

  handleEdit(): void {
    this.onEdit.emit(this.local.id);
  }

  handleDelete(): void {
    this.onDelete.emit(this.local.id);
  }
}