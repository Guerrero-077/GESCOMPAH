import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ButtonComponent } from "../button/button.component";
import { FormEstablishmentComponent } from '../../../features/establishments/components/form-establishment/form-establishment.component';
import { EstablishmentSelect } from '../../../features/establishments/models/establishment.models';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-card',
  imports: [ButtonComponent, CommonModule],
  templateUrl: './card.component.html',
  styleUrls: ['./card.component.css']
})
export class CardComponent {
  @Input() local!: EstablishmentSelect;
  @Input() showAvailableTag: boolean = true;

  @Output() onView = new EventEmitter<number>();
  @Output() onEdit = new EventEmitter<number>();
  @Output() onDelete = new EventEmitter<number>();
  @Output() onUpdate = new EventEmitter<void>(); // Nuevo evento para actualización

  constructor(private dialog: MatDialog) { }

get primaryImage(): string {
  const first: any = this.local?.images?.[0];
  const url = first?.filePath ?? first?.FilePath;

  if (typeof url === 'string' && url.startsWith('http')) {
    return url; // Cloudinary: úsala tal cual
  }

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

  handleView(): void {
    this.onView.emit(this.local.id);
  }

  handleEdit(): void {
    const dialogRef = this.dialog.open(FormEstablishmentComponent, {
      width: '800px',
      data: this.local // datos local a editar
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.onUpdate.emit(); // Emitir actualiza correctamente
      }
    });

    this.onEdit.emit(this.local.id); // Opcional: si aún necesitas este evento
  }

  handleDelete(): void {
    this.onDelete.emit(this.local.id);
  }
}
