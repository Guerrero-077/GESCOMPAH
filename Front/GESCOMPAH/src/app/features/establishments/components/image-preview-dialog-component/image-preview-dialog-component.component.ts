import { CommonModule } from '@angular/common';
import { Component, Inject, HostListener, signal } from '@angular/core';
import { MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { PreviewData } from '../../models/establishment.models';

@Component({
  selector: 'app-image-preview-dialog',
  imports: [CommonModule, MatDialogModule, MatIconModule, MatButtonModule],
  templateUrl: './image-preview-dialog-component.component.html',
  styleUrls: ['./image-preview-dialog-component.component.css']
})
export class ImagePreviewDialogComponent {
  sources: string[] = [];
  currentIndex = signal(0);

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: PreviewData,
    private ref: MatDialogRef<ImagePreviewDialogComponent>
  ) {
    this.sources = data?.sources ?? [];
    const start = Math.min(Math.max(0, data?.index ?? 0), Math.max(0, this.sources.length - 1));
    this.currentIndex.set(start);
  }

  @HostListener('document:keydown', ['$event'])
  onKey(e: KeyboardEvent) {
    if (e.key === 'Escape') this.close();
    if (e.key === 'ArrowRight') this.next(e);
    if (e.key === 'ArrowLeft') this.prev(e);
  }

  next(ev?: Event) {
    ev?.stopPropagation(); if (!this.sources.length) return;
    this.currentIndex.update(i => (i + 1) % this.sources.length);
  }
  prev(ev?: Event) {
    ev?.stopPropagation(); if (!this.sources.length) return;
    this.currentIndex.update(i => (i - 1 + this.sources.length) % this.sources.length);
  }
  close() { this.ref.close(); }
}
