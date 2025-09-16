import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { StandardButtonComponent } from '../../../../shared/components/standard-button/standard-button.component';

@Component({
  selector: 'app-establishment-detail-dialog',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, StandardButtonComponent],
  templateUrl: './establishment-detail-dialog.component.html',
  styleUrls: ['./establishment-detail-dialog.component.css']
})
export class EstablishmentDetailDialogComponent implements OnInit {


  currentImageIndex = 0;
  intervalId: any;

  constructor(
    public dialogRef: MatDialogRef<EstablishmentDetailDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any // Aquí se recibirá la información del establecimiento
  ) { }

  ngOnInit(): void {
    if (this.data.images?.length > 1) {
      this.startCarousel();
    }
  }

  ngOnDestroy(): void {
    this.clearCarousel();
  }

  startCarousel(): void {
    this.intervalId = setInterval(() => {
      this.nextImage();
    }, 3000);
  }

  clearCarousel(): void {
    if (this.intervalId) {
      clearInterval(this.intervalId);
    }
  }

  nextImage(): void {
    this.currentImageIndex = (this.currentImageIndex + 1) % this.data.images.length;
  }

  prevImage(): void {
    this.currentImageIndex =
      (this.currentImageIndex - 1 + this.data.images.length) % this.data.images.length;
  }

  goToImage(index: number): void {
    this.currentImageIndex = index;
    this.clearCarousel();
    this.startCarousel(); // reiniciar autoplay
  }

  onClose(): void {
    this.dialogRef.close();
  }

  // Método para abrir modal de imagen en tamaño completo
  viewImage(imagePath: string): void {
    // Implementar modal de imagen o lightbox
    window.open(imagePath, '_blank');
  }
}
