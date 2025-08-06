import { Component, EventEmitter, Input, Output } from "@angular/core";
import { ActionButtonComponenet } from "../../Button/action-button-componenet/action-button-componenet";
import { CommonModule } from "@angular/common";
import { EstablishmentSelect } from "../../../../features/Establishment/Models/Establishment.models";
import { MatDialog } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { EstablishmentFormComponent } from "../../../../features/Establishment/Pages/Form/establishment-form-component/establishment-form-component";

@Component({
  selector: 'app-local-card',
  standalone: true,
  imports: [
    ActionButtonComponenet,
    CommonModule,
    MatButtonModule
  ],
  templateUrl: './local-card.html',
  styleUrls: ['./local-card.css']
})
export class LocalCard {
  @Input() local!: EstablishmentSelect;
  @Input() showAvailableTag: boolean = true;

  @Output() onView = new EventEmitter<number>();
  @Output() onEdit = new EventEmitter<number>();
  @Output() onDelete = new EventEmitter<number>();
  @Output() onUpdate = new EventEmitter<void>(); // Nuevo evento para actualización

  constructor(private dialog: MatDialog) { }

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
    const dialogRef = this.dialog.open(EstablishmentFormComponent, {
      width: '800px',
      data: this.local // Pasamos los datos del local a editar
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.onUpdate.emit(); // Emitir evento cuando se actualiza correctamente
      }
    });

    this.onEdit.emit(this.local.id); // Opcional: si aún necesitas este evento
  }

  handleDelete(): void {
    this.onDelete.emit(this.local.id);
  }
}