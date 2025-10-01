import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { IonCard, IonCardContent, IonCardHeader, IonImg, IonLabel, IonGrid, IonRow, IonCol, IonText, IonButton, IonIcon } from '@ionic/angular/standalone';
import { CommonModule } from '@angular/common';
import { ButtonComponent } from '../button/button.component';

import { EstablishmentSelect } from 'src/app/feature/establishment/model/establishment.models';

@Component({
  selector: 'app-card',
  standalone: true,
  imports: [
    CommonModule,
    IonCard,
    IonCardContent,
    IonCardHeader,
    IonImg,
    IonLabel,
    IonGrid,
    IonRow,
    IonCol,
    IonText,
    ButtonComponent,
    IonButton,
    IonIcon,
],
  templateUrl: './card.component.html',
  styleUrls: ['./card.component.scss'],
})
export class CardComponent {
  @Input() local!: EstablishmentSelect;
  @Input() showAvailableTag = true;

  @Output() onView = new EventEmitter<number>();
  @Output() onEdit = new EventEmitter<number>();
  @Output() onDelete = new EventEmitter<number>();
  @Output() onUpdate = new EventEmitter<void>();

  get primaryImage(): string {
    const first: any = this.local?.images?.[0];
    const url = first?.filePath ?? first?.FilePath;
    return typeof url === 'string' && url.startsWith('http')
      ? url
      : 'assets/img/fallback.png';
  }

  get formattedRent(): string {
    return new Intl.NumberFormat('es-CO', {
      style: 'currency',
      currency: 'COP',
      minimumFractionDigits: 0,
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
