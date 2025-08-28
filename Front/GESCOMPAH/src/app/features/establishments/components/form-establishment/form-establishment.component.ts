import { CommonModule } from '@angular/common';
import { Component, Inject, Optional, HostListener } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatStepperModule } from '@angular/material/stepper';
import { EstablishmentCreate, EstablishmentSelect, EstablishmentUpdate, ImageSelectDto } from '../../models/establishment.models';
import { EstablishmentStore } from '../../services/establishment/establishment.store';
import { ImageService } from '../../services/image/image.service';
import { SquareService } from '../../services/square/square.service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { SquareSelectModel } from '../../models/squares.models';
import { ImagePreviewDialogComponent } from '../image-preview-dialog-component/image-preview-dialog-component.component';

// ========================
// Validadores
// ========================
function notOnlySpaces() {
  return (c: AbstractControl): ValidationErrors | null => {
    const v = c.value;
    // si está vacío, deja el "required" como único error
    if (v === null || v === undefined || v === '') return null;
    return String(v).trim().length === 0 ? { onlySpaces: true } : null;
  };
}

// Acepta "23.5" o "23,5" y limita a N decimales
function twoDecimals(max = 2) {
  return (c: AbstractControl): ValidationErrors | null => {
    const raw = c.value;
    if (raw === null || raw === undefined || raw === '') return null;

    const s = String(raw).trim().replace(',', '.'); // normaliza coma→punto
    // Debe lucir como número decimal básico
    if (!/^\d+(\.\d+)?$/.test(s)) return { NaN: true };

    const [, decimals] = s.split('.');
    if (decimals && decimals.length > max) return { decimalScale: { max } };
    return null;
  };
}

function numberRange(min?: number, max?: number) {
  return (c: AbstractControl): ValidationErrors | null => {
    const raw = c.value;
    if (raw === null || raw === undefined || raw === '') return null;
    const n = Number(String(raw).replace(',', '.')); // soporta coma
    if (Number.isNaN(n)) return { NaN: true };
    if (min !== undefined && n < min) return { min: { min, actual: n } };
    if (max !== undefined && n > max) return { max: { max, actual: n } };
    return null;
  };
}

function addressPattern() {
  const regex = /^[\p{L}\d\s#\-,.]+$/u;
  return (c: AbstractControl): ValidationErrors | null => {
    const v = (c.value ?? '') as string;
    if (!v) return null;
    return regex.test(v) ? null : { addressInvalid: true };
  };
}

@Component({
  selector: 'app-form-establishment',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatStepperModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './form-establishment.component.html',
  styleUrls: ['./form-establishment.component.css'],
})
export class FormEstablishmentComponent {
  generalGroup!: FormGroup;
  ubicacionGroup!: FormGroup;

  isEdit = false;
  isLoading = false;
  isDragging = false;
  isDeletingImage = false;

  readonly MAX_IMAGES = 5;
  readonly MAX_FILE_SIZE_MB = 5;
  readonly MAX_FILE_SIZE_BYTES = this.MAX_FILE_SIZE_MB * 1024 * 1024;

  selectedFiles: File[] = [];
  imagesPreview: string[] = [];              // nuevas (dataURL)
  existingImagesFull: ImageSelectDto[] = []; // existentes
  existingImagesToDelete: string[] = [];

  Squares: SquareSelectModel[] = [];

  readonly errors = {
    name: { required: 'El nombre es obligatorio.', maxlength: 'El nombre no puede superar 100 caracteres.', onlySpaces: 'El nombre no puede ser solo espacios.' },
    description: { required: 'La descripción es obligatoria.', maxlength: 'La descripción no puede superar 500 caracteres.', onlySpaces: 'La descripción no puede ser solo espacios.' },
    rentValueBase: { required: 'El valor base es obligatorio.', NaN: 'Ingresa un número válido.', min: 'El valor base no puede ser menor que 1.', max: 'El valor base es demasiado alto.', decimalScale: 'Máximo 2 decimales permitidos.' },
    uvtQty: { required: 'La cantidad de UVT es obligatoria.', NaN: 'Ingresa un número válido.', min: 'La cantidad UVT no puede ser menor que 1.', max: 'La cantidad UVT es demasiado alta.', decimalScale: 'Máximo 2 decimales permitidos.' },
    areaM2: { required: 'El área es obligatoria.', NaN: 'Ingresa un número válido.', min: 'El área no puede ser menor que 1 m².', max: 'El área es demasiado alta.', decimalScale: 'Máximo 2 decimales permitidos.' },
    plazaId: { required: 'La plaza es obligatoria.' },
    address: { maxlength: 'La dirección no puede superar 150 caracteres.', addressInvalid: 'Usa solo letras, números, espacios y # - , .' }
  } as const;

  constructor(
    private fb: FormBuilder,
    private store: EstablishmentStore,
    private imageService: ImageService,
    private plazasSrv: SquareService,
    private dialogRef: MatDialogRef<FormEstablishmentComponent>,
    private sweetAlert: SweetAlertService,
    private dialog: MatDialog,
    @Optional() @Inject(MAT_DIALOG_DATA) public data?: EstablishmentSelect
  ) {
    this.isEdit = !!data?.id;
  }

  ngOnInit(): void {
    this.startForms();
    if (this.isEdit) this.patchValues();
    this.loadPlazas();
  }

  private loadPlazas(): void {
    this.plazasSrv.getAll().subscribe({ next: (plazas) => (this.Squares = plazas) });
  }

  private startForms(): void {
    this.generalGroup = this.fb.group(
      {
        name: ['', [Validators.required, Validators.maxLength(100), notOnlySpaces()]],
        description: ['', [Validators.required, Validators.maxLength(500), notOnlySpaces()]],
        rentValueBase: [null, [Validators.required, numberRange(1, 9_999_999.99), twoDecimals(2)]],
        uvtQty: [null, [Validators.required, numberRange(1, 9999), twoDecimals(2)]],
        areaM2: [null, [Validators.required, numberRange(1, 1_000_000), twoDecimals(2)]],
      },
      { updateOn: 'blur' } // valida al salir del campo
    );

    this.ubicacionGroup = this.fb.group(
      {
        plazaId: [null, Validators.required],
        address: ['', [Validators.maxLength(150), addressPattern()]],
      },
      { updateOn: 'blur' }
    );
  }

  private patchValues(): void {
    if (!this.data) return;
    this.generalGroup.patchValue({
      name: this.data.name,
      description: this.data.description,
      areaM2: this.data.areaM2,
      rentValueBase: this.data.rentValueBase,
      uvtQty: this.data.uvtQty,
    });
    this.ubicacionGroup.patchValue({
      plazaId: this.data.plazaId,
      address: this.data.address ?? '',
    });
    this.existingImagesFull = [...this.data.images];
  }

  // --- Normalizaciones ---
  onTrim(control: AbstractControl | null) {
    if (!control) return;
    const v = (control.value ?? '') as string;
    const trimmed = v.trim().replace(/\s+/g, ' ');
    if (trimmed !== v) control.setValue(trimmed);
  }

  onNumberBlur(control: AbstractControl | null) {
    if (!control) return;
    const v = control.value;
    if (v === null || v === undefined || v === '') return;

    // Normaliza coma → punto, castea a number y revalida
    const s = String(v).replace(',', '.');
    const n = Number(s);
    if (!Number.isNaN(n)) control.setValue(n, { emitEvent: false });
    control.updateValueAndValidity({ emitEvent: false });
  }

  // ========================
  // Drag & Drop (prevención global)
  // ========================
  @HostListener('document:dragover', ['$event']) onDocumentDragOver(e: DragEvent){ e.preventDefault(); }
  @HostListener('document:drop', ['$event'])     onDocumentDrop(e: DragEvent){ e.preventDefault(); e.stopPropagation(); }

  onDragOver(e: DragEvent){ e.preventDefault(); e.stopPropagation(); this.isDragging = true; if (e.dataTransfer) e.dataTransfer.dropEffect = 'copy'; }
  onDragLeave(e: DragEvent){ e.preventDefault(); e.stopPropagation(); this.isDragging = false; }

  onFileDropped(e: DragEvent | FileList){
    if (e instanceof DragEvent){
      e.preventDefault(); e.stopPropagation(); this.isDragging = false;
      const files = e.dataTransfer?.files; if (!files || !files.length) return;
      this.processFiles(files); return;
    }
    const files = e as FileList; if (!files?.length) return;
    this.processFiles(files);
  }

  handleFileInput(ev: Event){
    const input = ev.target as HTMLInputElement;
    if (input.files?.length) this.onFileDropped(input.files);
  }

  private processFiles(files: FileList){
    const total = this.selectedFiles.length + this.existingImagesFull.length;
    const remaining = this.MAX_IMAGES - total;
    if (remaining <= 0) {
      this.sweetAlert.showNotification('Límite de imágenes', `Máximo ${this.MAX_IMAGES} imágenes permitidas`, 'warning');
      return;
    }

    const errors: string[] = [];
    const newFiles: File[] = [];

    Array.from(files).forEach((file) => {
      if (!file.type.startsWith('image/')) errors.push(`"${file.name}" no es una imagen válida.`);
      else if (file.size > this.MAX_FILE_SIZE_BYTES) errors.push(`"${file.name}" pesa ${(file.size / (1024 * 1024)).toFixed(2)} MB (máx. ${this.MAX_FILE_SIZE_MB})`);
      else if (newFiles.length < remaining) newFiles.push(file);
    });

    if (errors.length) this.sweetAlert.showNotification('Error en archivos', errors.join('\n'), 'error');

    this.selectedFiles.push(...newFiles);
    newFiles.forEach((file) => {
      const reader = new FileReader();
      reader.onload = (e: ProgressEvent<FileReader>) => { if (e.target?.result) this.imagesPreview.push(e.target.result as string); };
      reader.readAsDataURL(file);
    });
  }

  removeImage(index: number, isExisting: boolean){
    if (this.isDeletingImage) return;

    if (isExisting) {
      const image = this.existingImagesFull[index];
      this.sweetAlert.showConfirm('¿Eliminar esta imagen?', 'Esta acción no se puede deshacer')
        .then(result => {
          if (result.isConfirmed) {
            this.isDeletingImage = true;
            this.imageService.deleteImageById(image.id).subscribe({
              next: () => { this.existingImagesFull.splice(index, 1); this.isDeletingImage = false;
                this.sweetAlert.showNotification('Éxito', 'Imagen eliminada exitosamente', 'success'); },
              error: () => (this.isDeletingImage = false),
            });
          }
        });
    } else {
      this.selectedFiles.splice(index, 1);
      this.imagesPreview.splice(index, 1);
    }
  }

  // ========================
  // PREVIEW EN DIALOG
  // ========================
  private getCombinedGallery(clickedIndex: number, isExisting: boolean){
    const existingSources = this.existingImagesFull.map(x => x.filePath);
    const newSources = this.imagesPreview;
    const sources = [...existingSources, ...newSources];
    const index = isExisting ? clickedIndex : existingSources.length + clickedIndex;
    return { sources, index };
  }

  openPreviewFromExisting(i: number){
    const { sources, index } = this.getCombinedGallery(i, true);
    this.dialog.open(ImagePreviewDialogComponent, {
      data: { sources, index, title: 'Imágenes del local' },
      autoFocus: false,
      maxWidth: '100vw',
      width: '100vw',
      panelClass: 'img-preview-dialog'
    });
  }

  openPreviewFromNew(i: number){
    const { sources, index } = this.getCombinedGallery(i, false);
    this.dialog.open(ImagePreviewDialogComponent, {
      data: { sources, index, title: 'Imágenes del local' },
      autoFocus: false,
      maxWidth: '100vw',
      width: '100vw',
      panelClass: 'img-preview-dialog'
    });
  }

  // --- Acciones del formulario ---
  onSubmit(): void {
    if (this.generalGroup.invalid || this.ubicacionGroup.invalid) {
      this.markAllTouched();
      this.sweetAlert.showNotification('Campos incompletos', 'Completa todos los campos requeridos', 'warning');
      return;
    }

    if (this.selectedFiles.length === 0 && this.existingImagesFull.length === 0) {
      this.sweetAlert.showNotification('Sin imágenes', 'Debe agregar al menos una imagen', 'warning');
      return;
    }

    this.isLoading = true;
    const base = this.generalGroup.value;
    const loc = this.ubicacionGroup.value;

    const dto: EstablishmentCreate | EstablishmentUpdate = this.isEdit
      ? {
          id: this.data!.id,
          name: base.name,
          description: base.description,
          areaM2: base.areaM2,
          rentValueBase: base.rentValueBase,
          uvtQty: base.uvtQty,
          plazaId: loc.plazaId,
          address: loc.address,
          images: this.selectedFiles.length ? this.selectedFiles : undefined,
          imagesToDelete: this.existingImagesToDelete.length ? this.existingImagesToDelete : undefined,
        }
      : {
          name: base.name,
          description: base.description,
          areaM2: base.areaM2,
          rentValueBase: base.rentValueBase,
          uvtQty: base.uvtQty,
          plazaId: loc.plazaId,
          address: loc.address,
          files: this.selectedFiles.length ? this.selectedFiles : undefined,
        };

    const request$ = this.isEdit ? this.store.update(dto as EstablishmentUpdate) : this.store.create(dto as EstablishmentCreate);
    request$.subscribe({
      next: () => this.dialogRef.close(true),
      error: () => (this.isLoading = false),
    });
  }

  cancel(): void { this.dialogRef.close(false); }

  private markAllTouched(): void {
    this.generalGroup.markAllAsTouched();
    this.ubicacionGroup.markAllAsTouched();
  }
}
