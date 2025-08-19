import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-establishment-detail-dialog',
  standalone: true,
  imports: [CommonModule, MatButtonModule],
  templateUrl: './establishment-detail-dialog.component.html',
  styleUrls: ['./establishment-detail-dialog.component.css']
})
export class EstablishmentDetailDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<EstablishmentDetailDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any // Aquí se recibirá la información del establecimiento
  ) { }

  onClose(): void {
    this.dialogRef.close();
  }

  // Método para abrir modal de imagen en tamaño completo
  viewImage(imagePath: string): void {
    // Implementar modal de imagen o lightbox
    console.log('Abrir imagen:', imagePath);
    window.open(imagePath, '_blank');
  }
}
