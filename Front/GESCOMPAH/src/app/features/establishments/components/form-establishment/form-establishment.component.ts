import { CommonModule } from '@angular/common';
import { Component, Inject, Optional } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatStepperModule } from '@angular/material/stepper';
import { EstablishmentCreate, EstablishmentSelect, EstablishmentUpdate, ImageSelectDto } from '../../models/establishment.models';
import { SquareModel } from '../../models/squares.models';
import { EstablishmentStore } from '../../services/establishment/establishment.store';
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
        MatProgressSpinnerModule,
    ],
    templateUrl: './form-establishment.component.html',
    styleUrl: './form-establishment.component.css',
})
export class FormEstablishmentComponent {
    // Formularios
    generalGroup!: FormGroup;
    ubicacionGroup!: FormGroup;

    // Estado de la UI
    isEdit = false;
    isLoading = false;
    isDragging = false;
    isDeletingImage = false;

    // Constantes
    readonly MAX_IMAGES = 5;
    readonly MAX_FILE_SIZE_MB = 5;
    readonly MAX_FILE_SIZE_BYTES = this.MAX_FILE_SIZE_MB * 1024 * 1024;

    // Gestión de imágenes
    selectedFiles: File[] = [];
    imagesPreview: string[] = [];
    existingImagesFull: ImageSelectDto[] = [];
    existingImagesToDelete: string[] = [];

    // Datos de soporte
    Squares: SquareModel[] = [];

    constructor(
        private fb: FormBuilder,
        private store: EstablishmentStore,
        private imageService: ImageService,
        private plazasSrv: SquareService,
        private dialogRef: MatDialogRef<FormEstablishmentComponent>,
        private snackBar: MatSnackBar,
        @Optional() @Inject(MAT_DIALOG_DATA) public data?: EstablishmentSelect
    ) {
        this.isEdit = !!data?.id;
    }

    ngOnInit(): void {
        this.startForms();
        if (this.isEdit) {
            this.patchValues();
        }
        this.loadPlazas();
    }

    private loadPlazas(): void {
        this.plazasSrv
            .getAll('Plaza')
            .subscribe({ next: (plazas) => (this.Squares = plazas) });
    }

    private startForms(): void {
        this.generalGroup = this.fb.group({
            name: ['', [Validators.required, Validators.maxLength(100)]],
            description: ['', [Validators.required, Validators.maxLength(500)]],
            rentValueBase: [0, [Validators.required, Validators.min(0)]],
            areaM2: [0, [Validators.required, Validators.min(0)]],
        });

        this.ubicacionGroup = this.fb.group({
            plazaId: [null, Validators.required],
            address: [''],
        });
    }

    private patchValues(): void {
        if (!this.data) return;

        this.generalGroup.patchValue({
            name: this.data.name,
            description: this.data.description,
            areaM2: this.data.areaM2,
            rentValueBase: this.data.rentValueBase,
        });

        this.ubicacionGroup.patchValue({
            plazaId: this.data.plazaId,
            address: this.data.address ?? '',
        });

        this.existingImagesFull = [...this.data.images];
    }

    // --- Métodos para Drag & Drop ---
    onDragOver(e: DragEvent): void {
        e.preventDefault();
        e.stopPropagation();
        this.isDragging = true;
    }

    onDragLeave(e: DragEvent): void {
        e.preventDefault();
        e.stopPropagation();
        this.isDragging = false;
    }

    onFileDropped(e: DragEvent | FileList): void {
        this.isDragging = false;
        const files = e instanceof DragEvent ? e.dataTransfer?.files : (e as FileList);
        if (!files) return;
        this.processFiles(files);
    }

    handleFileInput(ev: Event): void {
        const input = ev.target as HTMLInputElement;
        if (input.files?.length) {
            this.onFileDropped(input.files);
        }
    }

    private processFiles(files: FileList): void {
        const totalImages = this.selectedFiles.length + this.existingImagesFull.length;
        const remainingSlots = this.MAX_IMAGES - totalImages;
        if (remainingSlots <= 0) {
            this.snackBar.open(`Máximo ${this.MAX_IMAGES} imágenes permitidas`, 'Cerrar', { duration: 3000 });
            return;
        }

        const errors: string[] = [];
        const newFiles: File[] = [];

        Array.from(files).forEach((file) => {
            if (!file.type.startsWith('image/')) {
                errors.push(`"${file.name}" no es una imagen válida.`);
            } else if (file.size > this.MAX_FILE_SIZE_BYTES) {
                errors.push(`"${file.name}" pesa ${(file.size / (1024 * 1024)).toFixed(2)} MB (máx. ${this.MAX_FILE_SIZE_MB})`);
            } else if (newFiles.length < remainingSlots) {
                newFiles.push(file);
            }
        });

        if (errors.length) {
            this.snackBar.open(errors.join('\n'), 'Cerrar', { duration: 6000 });
        }

        this.selectedFiles.push(...newFiles);

        newFiles.forEach((file) => {
            const reader = new FileReader();
            reader.onload = (e: ProgressEvent<FileReader>) => {
                if (e.target?.result) {
                    this.imagesPreview.push(e.target.result as string);
                }
            };
            reader.readAsDataURL(file);
        });
    }

    removeImage(index: number, isExisting: boolean): void {
        if (this.isDeletingImage) return;

        if (isExisting) {
            const image = this.existingImagesFull[index];
            if (!confirm('¿Eliminar esta imagen permanentemente?')) return;

            this.isDeletingImage = true;
            this.imageService.logicalDeleteImage(image.publicId).subscribe({
                next: () => {
                    this.existingImagesFull.splice(index, 1);
                    this.isDeletingImage = false;
                },
                error: () => (this.isDeletingImage = false),
            });
        } else {
            this.selectedFiles.splice(index, 1);
            this.imagesPreview.splice(index, 1);
        }
    }

    // --- Acciones del formulario ---
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
                imagesToDelete: this.existingImagesToDelete.length ? this.existingImagesToDelete : undefined,
            }
            : {
                name: base.name,
                description: base.description,
                areaM2: base.areaM2,
                rentValueBase: base.rentValueBase,
                plazaId: loc.plazaId,
                address: loc.address,
                files: this.selectedFiles.length ? this.selectedFiles : undefined,
            };

        const request$ = this.isEdit
            ? this.store.update(dto as EstablishmentUpdate)
            : this.store.create(dto as EstablishmentCreate);

        request$.subscribe({
            next: () => this.dialogRef.close(true),
            error: () => (this.isLoading = false),
        });
    }

    cancel(): void {
        this.dialogRef.close(false);
    }

    private markAllTouched(): void {
        this.generalGroup.markAllAsTouched();
        this.ubicacionGroup.markAllAsTouched();
    }
}