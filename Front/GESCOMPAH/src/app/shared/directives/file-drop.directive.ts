import { Directive, EventEmitter, HostBinding, HostListener, Input, Output } from '@angular/core';

@Directive({
  selector: '[appFileDrop]',
  standalone: true,
})
export class FileDropDirective {
  @Input() disabled = false;

  @Output() files = new EventEmitter<File[]>();
  @Output() dragOverChange = new EventEmitter<boolean>();

  @HostBinding('class.drag-over') isOver = false;

  // Previene que el navegador abra el archivo al soltar fuera del dropzone
  @HostListener('document:dragover', ['$event'])
  onDocumentDragOver(e: DragEvent) { e.preventDefault(); }

  @HostListener('document:drop', ['$event'])
  onDocumentDrop(e: DragEvent) { e.preventDefault(); e.stopPropagation(); }

  @HostListener('dragover', ['$event'])
  onDragOver(e: DragEvent) {
    if (this.disabled) return;
    e.preventDefault();
    e.stopPropagation();
    this.isOver = true;
    this.dragOverChange.emit(true);
    if (e.dataTransfer) e.dataTransfer.dropEffect = 'copy';
  }

  @HostListener('dragleave', ['$event'])
  onDragLeave(e: DragEvent) {
    if (this.disabled) return;
    e.preventDefault();
    e.stopPropagation();
    this.isOver = false;
    this.dragOverChange.emit(false);
  }

  @HostListener('drop', ['$event'])
  onDrop(e: DragEvent) {
    if (this.disabled) return;
    e.preventDefault();
    e.stopPropagation();
    this.isOver = false;
    this.dragOverChange.emit(false);

    const list = e.dataTransfer?.files;
    if (!list || list.length === 0) return;

    const arr: File[] = [];
    for (let i = 0; i < list.length; i++) {
      const item = list.item(i);
      if (item) arr.push(item);
    }
    this.files.emit(arr);
  }
}
