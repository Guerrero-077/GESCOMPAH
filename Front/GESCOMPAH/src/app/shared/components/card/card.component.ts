import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ButtonComponent } from "../button/button.component";
import { EstablishmentCard, EstablishmentSelect } from '../../../features/establishments/models/establishment.models';
import { NgOptimizedImage } from '@angular/common';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-card',
  imports: [ButtonComponent, CommonModule, NgOptimizedImage],
  templateUrl: './card.component.html',
  styleUrls: ['./card.component.css']
})
export class CardComponent {
  // Acepta tanto el DTO completo como el liviano de cards
  @Input() local!: EstablishmentSelect | EstablishmentCard;
  @Input() showAvailableTag: boolean = true;

  @Output() onView = new EventEmitter<number>();
  @Output() onEdit = new EventEmitter<number>();
  @Output() onDelete = new EventEmitter<number>();
  @Output() onUpdate = new EventEmitter<void>(); // Nuevo evento para actualización

  constructor() { }

get primaryImage(): string {
  const anyLocal: any = this.local as any;
  const cardUrl = anyLocal?.primaryImagePath;
  if (typeof cardUrl === 'string' && cardUrl.length > 0) return cardUrl;
  const first: any = anyLocal?.images?.[0];
  const url = first?.filePath ?? first?.FilePath;
  if (typeof url === 'string' && url.length > 0) return url;
  // Fallback transparente (sin 404)
  return 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/wwAAn8B9oZ1VwAAAABJRU5ErkJggg==';
}


  get formattedRent(): string {
    return new Intl.NumberFormat('es-CO', {
      style: 'currency',
      currency: 'COP',
      minimumFractionDigits: 0
    }).format(this.local?.rentValueBase || 0);
  }

  // Descripción truncada con fallback
  get shortDescription(): string {
    const anyLocal: any = this.local as any;
    const desc: string = (anyLocal?.description ?? '').toString().trim();
    if (!desc) return '';
    const max = 110; // ~2 líneas en el ancho definido
    return desc.length > max ? (desc.slice(0, max).trim() + '…') : desc;
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
