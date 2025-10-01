import { CommonModule } from '@angular/common';
import { Component, HostListener, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { StandardButtonComponent } from '../../../../shared/components/standard-button/standard-button.component';

export interface ImagePreviewData {
  /** Título opcional del visor */
  title?: string;
  /** Modo simple: una sola imagen */
  imageSrc?: string;
  /** Modo carrusel: lista completa de imágenes */
  imageList?: string[];
  /** Índice inicial dentro de imageList */
  startIndex?: number;
}

@Component({
  selector: 'app-image-preview-dialog',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, StandardButtonComponent],
  templateUrl: './image-preview-dialog-component.component.html',
  styleUrls: ['./image-preview-dialog-component.component.css'],
})
export class ImagePreviewDialogComponent {
  readonly sources: string[];
  index: number;

  constructor(
    private readonly ref: MatDialogRef<ImagePreviewDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ImagePreviewData
  ) {
    const list = (data?.imageList ?? [])
      .filter((s) => !!s && typeof s === 'string');

    this.sources = list.length > 0
      ? list
      : (data?.imageSrc ? [data.imageSrc] : []);

    const start = data?.startIndex ?? 0;
    this.index = this.clamp(start, 0, Math.max(this.sources.length - 1, 0));
  }

  get title(): string {
    return this.data?.title ?? 'Vista previa';
  }

  get total(): number {
    return this.sources.length;
  }

  get current(): number {
    return this.index + 1;
  }

  get currentSrc(): string {
    return this.sources[this.index] ?? '';
  }

  close(): void {
    this.ref.close();
  }

  next(): void {
    if (!this.total) return;
    this.index = (this.index + 1) % this.total;
  }

  prev(): void {
    if (!this.total) return;
    this.index = (this.index - 1 + this.total) % this.total;
  }

  @HostListener('window:keydown', ['$event'])
  onKey(e: KeyboardEvent) {
    if (!this.total) return;
    if (e.key === 'ArrowRight') { this.next(); e.preventDefault(); }
    else if (e.key === 'ArrowLeft') { this.prev(); e.preventDefault(); }
    else if (e.key === 'Escape') { this.close(); }
  }

  private clamp(n: number, min: number, max: number): number {
    return Math.min(Math.max(n, min), max);
  }
}
