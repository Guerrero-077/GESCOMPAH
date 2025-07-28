import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Inject, Optional, Output } from '@angular/core';
import { ReactiveFormsModule, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar } from '@angular/material/snack-bar';
import { EstablishmentSelect, EstablishmentCreate, EstablishmentUpdate, ExistingImage } from '../../../Models/Establishment.models';
import { LocalesService } from '../../../Services/Locales/locales-service';
import { MatSelectModule } from '@angular/material/select';
import { MatStepperModule } from '@angular/material/stepper';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PlazaModel } from '../../../Models/Plaza.models';
import { PlazasService } from '../../../Services/Plazas/plazas-service';
import { ImageService } from '../../../Services/Images/image-service';

@Component({
  selector: 'app-establishment-form-component',
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
    MatProgressSpinnerModule
  ],
  templateUrl: './establishment-form-component.html',
  styleUrls: ['./establishment-form-component.css']
})
export class EstablishmentFormComponent {
  generalGroup!: FormGroup;
  ubicacionGroup!: FormGroup;

  isEdit = false;
  isLoading = false;
  isDragging = false;
  isDeletingImage = false;

  selectedFiles: File[] = [];
  imagesPreview: string[] = [];

  existingImagesFull: ExistingImage[] = [];
  existingImages: string[] = [];

  plazas: PlazaModel[] = [];

  @Output() refreshList = new EventEmitter<void>();

  constructor(
    private fb: FormBuilder,
    private establishmentService: LocalesService,
    private imageService: ImageService,
    private plazasService: PlazasService,
    private dialogRef: MatDialogRef<EstablishmentFormComponent>,
    private snackBar: MatSnackBar,
    @Optional() @Inject(MAT_DIALOG_DATA) public data?: EstablishmentSelect
  ) {
    this.isEdit = !!data?.id;
  }

  ngOnInit(): void {
    this.initForms();
    this.patchFormValues();
    this.loadPlazas();
  }

  private loadPlazas(): void {
    this.plazasService.getAll().subscribe({
      next: (plazas) => {
        this.plazas = plazas;
      },
      error: (err) => {
        console.error('Error loading plazas:', err);
        this.snackBar.open('Error al cargar las plazas', 'Cerrar', { duration: 3000 });
      }
    });
  }

  private initForms(): void {
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

  private patchFormValues(): void {
    if (this.isEdit && this.data) {
      this.generalGroup.patchValue({
        name: this.data.name,
        description: this.data.description,
        rentValueBase: this.data.rentValueBase,
        areaM2: this.data.areaM2
      });

      this.ubicacionGroup.patchValue({
        plazaId: this.data.plazaId,
        address: this.data.address || ''
      });

      this.existingImagesFull = this.data.images.map(img => ({
        id: img.id,
        fileName: img.fileName,
        filePath: img.filePath,
        publicId: img.publicId || ''
      }));

      this.existingImages = this.data.images.map(img => img.filePath);
    }
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = true;
  }

  onDragLeave(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;
  }

  onFileDropped(event: DragEvent | FileList): void {
    this.isDragging = false;

    let files: FileList;

    if (event instanceof DragEvent) {
      event.preventDefault();
      if (!event.dataTransfer?.files) return;
      files = event.dataTransfer.files;
    } else {
      files = event;
    }

    this.processFiles(files);
  }

  handleFileInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.onFileDropped(input.files);
    }
  }

  private processFiles(files: FileList): void {
    const totalImages = this.selectedFiles.length + this.existingImages.length;
    const remainingSlots = 5 - totalImages;

    if (remainingSlots <= 0) {
      this.snackBar.open('Máximo 5 imágenes permitidas', 'Cerrar', { duration: 3000 });
      return;
    }

    for (let i = 0; i < Math.min(remainingSlots, files.length); i++) {
      const file = files[i];
      if (!file.type.match(/image\/*/)) continue;

      this.selectedFiles.push(file);
      const reader = new FileReader();
      reader.onload = (e: ProgressEvent<FileReader>) => {
        if (e.target?.result) {
          this.imagesPreview.push(e.target.result as string);
        }
      };
      reader.readAsDataURL(file);
    }
  }

  removeImage(index: number, isExisting: boolean): void {
    if (this.isDeletingImage) return;

    if (isExisting) {
      const imageToDelete = this.existingImagesFull[index];

      if (!confirm('¿Estás seguro de eliminar esta imagen permanentemente?')) {
        return;
      }

      this.isDeletingImage = true;

      this.imageService.deleteImageById(imageToDelete.id).subscribe({
        next: () => {
          // Encontrar el índice exacto por ID por seguridad
          const exactIndex = this.existingImagesFull.findIndex(img => img.id === imageToDelete.id);
          if (exactIndex > -1) {
            this.existingImages.splice(exactIndex, 1);
            this.existingImagesFull.splice(exactIndex, 1);
          }
          this.snackBar.open('Imagen eliminada correctamente', 'Cerrar', { duration: 3000 });
          this.isDeletingImage = false;
        },
        error: (err) => {
          console.error('Error al eliminar la imagen:', err);
          this.snackBar.open('Error al eliminar la imagen', 'Cerrar', { duration: 3000 });
          this.isDeletingImage = false;
        }
      });
    } else {
      // Para imágenes nuevas que no se han subido aún
      this.selectedFiles.splice(index, 1);
      this.imagesPreview.splice(index, 1);
      this.snackBar.open('Imagen removida', 'Cerrar', { duration: 2000 });
    }
  }

  onSubmit(): void {
    if (this.generalGroup.invalid || this.ubicacionGroup.invalid) {
      this.markAllAsTouched();
      this.snackBar.open('Por favor complete todos los campos requeridos', 'Cerrar', { duration: 3000 });
      return;
    }

    if (this.selectedFiles.length === 0 && this.existingImagesFull.length === 0) {
      this.snackBar.open('Debe agregar al menos una imagen', 'Cerrar', { duration: 3000 });
      return;
    }

    this.isLoading = true;

    const dto: EstablishmentCreate | EstablishmentUpdate = this.isEdit
      ? {
        id: this.data!.id,
        name: this.generalGroup.value.name,
        description: this.generalGroup.value.description,
        areaM2: this.generalGroup.value.areaM2,
        rentValueBase: this.generalGroup.value.rentValueBase,
        plazaId: this.ubicacionGroup.value.plazaId,
        address: this.ubicacionGroup.value.address,
        files: this.selectedFiles.length > 0 ? this.selectedFiles : undefined,
        existingImages: this.existingImagesFull.map(img => ({
          id: img.id,
          fileName: img.fileName,
          filePath: img.filePath,
          publicId: img.publicId
        }))
      }
      : {
        name: this.generalGroup.value.name,
        description: this.generalGroup.value.description,
        areaM2: this.generalGroup.value.areaM2,
        rentValueBase: this.generalGroup.value.rentValueBase,
        plazaId: this.ubicacionGroup.value.plazaId,
        address: this.ubicacionGroup.value.address,
        files: this.selectedFiles.length > 0 ? this.selectedFiles : undefined
      };

    const request$ = this.isEdit
      ? this.establishmentService.update(dto as EstablishmentUpdate)
      : this.establishmentService.create(dto as EstablishmentCreate);

    request$.subscribe({
      next: () => {
        this.snackBar.open(`Establecimiento ${this.isEdit ? 'actualizado' : 'creado'} correctamente`, 'Cerrar', { duration: 3000 });
        this.dialogRef.close(true);
      },
      error: (err) => {
        console.error('Error al guardar:', err);
        this.snackBar.open('Error al guardar el establecimiento', 'Cerrar', { duration: 3000 });
        this.isLoading = false;
      }
    });
  }

  private markAllAsTouched(): void {
    this.generalGroup.markAllAsTouched();
    this.ubicacionGroup.markAllAsTouched();
  }

  cancel(): void {
    this.dialogRef.close(false);
  }
}