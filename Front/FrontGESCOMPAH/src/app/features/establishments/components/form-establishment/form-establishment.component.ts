import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Inject, Optional, Output } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatStepperModule } from '@angular/material/stepper';
import { ImageSelectDto, EstablishmentSelect, EstablishmentCreate, EstablishmentUpdate } from '../../models/establishment.models';
import { SquareModel } from '../../models/squares.models';
import { EstablishmentService } from '../../services/establishment/establishment.service';
import { ImageService } from '../../services/image/image.service';
import { SquareService } from '../../services/square/square.service';

@Component({
  selector: 'app-form-establishment',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatStepperModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './form-establishment.component.html',
  styleUrl: './form-establishment.component.css'
})
export class FormEstablishmentComponent {
  /* ---------------------------------------------------------------------- */
  /* 1️⃣   Formularios  */
  /* ---------------------------------------------------------------------- */
  generalGroup!: FormGroup;
  ubicacionGroup!: FormGroup;

  /* ---------------------------------------------------------------------- */
  /* 2️⃣   Estado de la UI  */
  /* ---------------------------------------------------------------------- */
  isEdit = false;                        // `true` cuando se edita un establecimiento
  isLoading = false;                     // bloquea UI mientras se procesa
  isDragging = false;                    // cursor 'drag over'
  isDeletingImage = false;                // impide paralelismos

  readonly MAX_IMAGES = 5;
  readonly MAX_FILE_SIZE_MB = 5;
  readonly MAX_FILE_SIZE_BYTES = this.MAX_FILE_SIZE_MB * 1024 * 1024;

  /* ----------  Imagenes nuevas (no subidas todavía) ---------- */
  selectedFiles: File[] = [];
  imagesPreview: string[] = [];          // preview de las nuevas

  /* ----------  Imagenes existentes en la BD ---------- */
  existingImagesFull: ImageSelectDto[] = [];   // objetos con filePath, publicId ...
  existingImagesToDelete: string[] = [];       // publicIds que el usuario marcó

  Squares: SquareModel[] = [];

  /* ---------------------------------------------------------------------- */
  /* 3️⃣   Salida del componente  */
  /* ---------------------------------------------------------------------- */
  @Output() refreshList = new EventEmitter<void>();

  /* ---------------------------------------------------------------------- */
  /* 4️⃣   Constructor  */
  /* ---------------------------------------------------------------------- */
  constructor(
    private fb: FormBuilder,
    private estService: EstablishmentService,
    private imageService: ImageService,
    private plazasSrv: SquareService,
    private dialogRef: MatDialogRef<FormEstablishmentComponent>,
    private snackBar: MatSnackBar,
    @Optional() @Inject(MAT_DIALOG_DATA) public data?: EstablishmentSelect
  ) {
    this.isEdit = !!data?.id;
  }

  /* ---------------------------------------------------------------------- */
  /* 5️⃣   Ciclo de vida  */
  /* ---------------------------------------------------------------------- */
  ngOnInit(): void {
    this.startForms();
    if (this.isEdit) this.patchValues();
    this.loadPlazas();
  }

  /* ---------------------------------------------------------------------- */
  /* 6️⃣   Cargar Plaza  */
  /* ---------------------------------------------------------------------- */
  private loadPlazas(): void {
    this.plazasSrv.getAll("Plaza").subscribe({
      next: (plazas) => this.Squares = plazas,
      error: () => this.snackBar.open('Error al cargar plazas', 'Cerrar', { duration: 3000 })
    });
  }

  /* ---------------------------------------------------------------------- */
  /* 7️⃣   Inicializar FormGroup  */
  /* ---------------------------------------------------------------------- */
  private startForms(): void {
    this.generalGroup = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.required, Validators.maxLength(500)]],
      rentValueBase: [0, [Validators.required, Validators.min(0)]],
      areaM2: [0, [Validators.required, Validators.min(0)]]
    });

    this.ubicacionGroup = this.fb.group({
      plazaId: [null, Validators.required],
      address: ['']
    });
  }

  /* ---------------------------------------------------------------------- */
  /* 8️⃣   Rellenar el formulario con datos existentes  */
  /* ---------------------------------------------------------------------- */
  private patchValues(): void {
    if (!this.data) return;
    // Campos de negocio
    this.generalGroup.patchValue({
      name: this.data.name,
      description: this.data.description,
      areaM2: this.data.areaM2,
      rentValueBase: this.data.rentValueBase
    });

    this.ubicacionGroup.patchValue({
      plazaId: this.data.plazaId,
      address: this.data.address ?? ''
    });

    // Imagenes existentes
    this.existingImagesFull = [...this.data.images];
  }

  /* ---------------------------------------------------------------------- */
  /* 9️⃣   Drag & Drop  */
  /* ---------------------------------------------------------------------- */
  onDragOver(e: DragEvent): void { e.preventDefault(); e.stopPropagation(); this.isDragging = true; }
  onDragLeave(e: DragEvent): void { e.preventDefault(); e.stopPropagation(); this.isDragging = false; }

  onFileDropped(e: DragEvent | FileList): void {
    this.isDragging = false;

    const fdList = e instanceof DragEvent
      ? e.dataTransfer?.files
      : (e as FileList);

    if (!fdList) return;
    this.processFiles(fdList);
  }

  handleFileInput(ev: Event): void {
    const input = ev.target as HTMLInputElement;
    if (input.files && input.files.length) this.onFileDropped(input.files);
  }

  /** Limitar a 5 imágenes, validar tipo, tamaño y generar la preview */
  private processFiles(files: FileList): void {
    const totalImages = this.selectedFiles.length + this.existingImagesFull.length;
    const remainingSlots = this.MAX_IMAGES - totalImages;
    if (remainingSlots <= 0) {
      this.snackBar.open(`Máximo ${this.MAX_IMAGES} imágenes permitidas`, 'Cerrar', { duration: 3000 });
      return;
    }

    const errs: string[] = [];
    const newFiles: File[] = [];

    Array.from(files).forEach(file => {
      if (!file.type.startsWith('image/')) errs.push(`"${file.name}" no es una imagen válida.`);
      else if (file.size > this.MAX_FILE_SIZE_BYTES)
        errs.push(`"${file.name}" pesa ${(file.size / (1024 * 1024)).toFixed(2)} MB (máx. ${this.MAX_FILE_SIZE_MB})`);
      else if (newFiles.length < remainingSlots) // evitar exceder límite
        newFiles.push(file);
    });

    if (errs.length) this.snackBar.open(errs.join('\n'), 'Cerrar', { duration: 6000 });

    // Agregar sólo las nuevas
    this.selectedFiles.push(...newFiles);

    // Solo generar previews para las nuevas
    newFiles.forEach(file => {
      const reader = new FileReader();
      reader.onload = (e: ProgressEvent<FileReader>) => {
        if (e.target?.result) this.imagesPreview.push(e.target.result as string);
      };
      reader.readAsDataURL(file);
    });
  }


  /* ---------------------------------------------------------------------- */
  /* 🔄   Eliminar imagen (nueva o existente) */
  /* ---------------------------------------------------------------------- */
  removeImage(idx: number, isExisting: boolean): void {
    if (this.isDeletingImage) return;

    if (isExisting) {
      const img = this.existingImagesFull[idx];

      if (!confirm('¿Eliminar esta imagen permanentemente?')) return;

      this.isDeletingImage = true;

      this.imageService.logicalDeleteImage(img.publicId).subscribe({
        next: () => {
          this.existingImagesFull.splice(idx, 1);
          this.isDeletingImage = false;
        },
        error: (err) => {
          console.error('Error eliminando imagen', err);
          alert('No se pudo eliminar la imagen.');
          this.isDeletingImage = false;
        }
      });
    } else {
      this.selectedFiles.splice(idx, 1);
      this.imagesPreview.splice(idx, 1);
    }
  }




  /* ---------------------------------------------------------------------- */
  /* 🎯   Enviar datos al backend */
  /* ---------------------------------------------------------------------- */
  onSubmit(): void {
    if (this.generalGroup.invalid || this.ubicacionGroup.invalid) {
      this.markAllTouched();
      this.snackBar.open('Completa todos los campos requeridos', 'Cerrar', { duration: 3000 });
      return;
    }

    if (this.selectedFiles.length === 0 && this.existingImagesFull.length === 0) {
      this.snackBar.open('Debe agregar al menos una imagen', 'Cerrar', { duration: 3000 });
      return;
    }

    this.isLoading = true;

    const base = this.generalGroup.value;
    const loc = this.ubicacionGroup.value;

    /* ----  Construir DTO de acuerdo al mode de edición  ---- */
    const dto: EstablishmentCreate | EstablishmentUpdate = this.isEdit
      ? {
        id: this.data!.id,
        name: base.name,
        description: base.description,
        areaM2: base.areaM2,
        rentValueBase: base.rentValueBase,
        plazaId: loc.plazaId,
        address: loc.address,
        images: this.selectedFiles.length ? this.selectedFiles : undefined,
        imagesToDelete: this.existingImagesToDelete.length ? this.existingImagesToDelete : undefined
      }
      : {
        name: base.name,
        description: base.description,
        areaM2: base.areaM2,
        rentValueBase: base.rentValueBase,
        plazaId: loc.plazaId,
        address: loc.address,
        files: this.selectedFiles.length ? this.selectedFiles : undefined
      };

    /* ----  Llamada al servicio  ---- */
    const request$ = this.isEdit
      ? this.estService.update(dto as EstablishmentUpdate)
      : this.estService.create(dto as EstablishmentCreate);

    request$.subscribe({
      next: () => {
        this.snackBar.open(`Establecimiento ${this.isEdit ? 'actualizado' : 'creado'} correctamente`, 'Cerrar', { duration: 3000 });
        this.dialogRef.close(true);
      },
      error: err => {
        this.snackBar.open(`Error al ${this.isEdit ? 'actualizar' : 'crear'} el establecimiento`, 'Cerrar', { duration: 3000 });
        console.error(err);
        this.isLoading = false;
      }
    });
  }

  /* ---------------------------------------------------------------------- */
  /* Helpers  */
  /* ---------------------------------------------------------------------- */
  private markAllTouched(): void {
    this.generalGroup.markAllAsTouched();
    this.ubicacionGroup.markAllAsTouched();
  }

  cancel(): void { this.dialogRef.close(false); }
}