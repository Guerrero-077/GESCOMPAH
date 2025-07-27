import { CommonModule } from '@angular/common';
import { Component, Inject, Optional } from '@angular/core';
import { ReactiveFormsModule, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar } from '@angular/material/snack-bar';
import { EstablishmentSelect, EstablishmentCreate } from '../../../Models/Establishment.models';
import { LocalesService } from '../../../Services/Locales/locales-service';
import { PlazasService } from '../../../Services/Plazas/plazas-service';
import { PlazaModel } from '../../../Models/Plaza.models';
import { MatSelectModule } from '@angular/material/select';
import { MatStepperModule } from '@angular/material/stepper';
import { ListEstablishmentComponent } from '../../list-establishment-component/list-establishment-component';

@Component({
  selector: 'app-establishment-form-component',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatStepperModule
  ],
  templateUrl: './establishment-form-component.html',
  styleUrl: './establishment-form-component.css',

})
export class EstablishmentFormComponent {

  form!: FormGroup;
  plazas: PlazaModel[] = [];
  selectedFiles: File[] = [];
  imagesPreview: string[] = [];

  isEdit = false;
  isDragging = false;
  hoveredImageIndex: number | null = null;

  constructor(
    private fb: FormBuilder,
    private snackbar: MatSnackBar,
    private localesService: LocalesService,
    private plazaService: PlazasService,
    @Optional() private dialogRef?: MatDialogRef<EstablishmentFormComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) public data?: EstablishmentCreate
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      general: this.fb.group({
        name: [this.data?.name || '', Validators.required],
        description: [this.data?.description || '', Validators.required],
        areaM2: [this.data?.areaM2 || 0, [Validators.required, Validators.min(1)]],
        rentValueBase: [this.data?.rentValueBase || 0, [Validators.required, Validators.min(1)]]
      }),
      ubicacion: this.fb.group({
        plazaId: [this.data?.plazaId || null, Validators.required],
        address: [this.data?.['address'] || '', Validators.required] // si address no está en el modelo, agrégalo
      }),
      imagenes: this.fb.group({})
    });

    this.isEdit = !!this.data;
    this.loadPlazas();
  }

  get generalGroup(): FormGroup {
    return this.form.get('general') as FormGroup;
  }

  get ubicacionGroup(): FormGroup {
    return this.form.get('ubicacion') as FormGroup;
  }

  get imagenesGroup(): FormGroup {
    return this.form.get('imagenes') as FormGroup;
  }

  loadPlazas(): void {
    this.plazaService.getAll().subscribe({
      next: (res) => (this.plazas = res),
      error: (err) => this.snackbar.open('Error al cargar plazas: ' + err.message, 'Cerrar')
    });
  }

  /** Manejo manual */
  onFileChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files) {
      const files = Array.from(input.files);
      this.addFiles(files);
    }
  }

  /** Drag & drop */
  onDragOver(event: DragEvent): void {
    event.preventDefault();
    this.isDragging = true;
  }

  onDragLeave(event: DragEvent): void {
    event.preventDefault();
    this.isDragging = false;
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    this.isDragging = false;

    if (event.dataTransfer?.files) {
      const files = Array.from(event.dataTransfer.files);
      this.addFiles(files);
    }
  }

  addFiles(files: File[]): void {
    const remaining = 5 - this.selectedFiles.length;
    const toAdd = files.slice(0, remaining);

    this.selectedFiles.push(...toAdd);
    this.imagesPreview.push(...toAdd.map(f => URL.createObjectURL(f)));
  }

  removeImage(index: number): void {
    this.selectedFiles.splice(index, 1);
    this.imagesPreview.splice(index, 1);
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    const dto: EstablishmentCreate = {
      ...this.generalGroup.value,
      ...this.ubicacionGroup.value,
      files: this.selectedFiles,
      id: this.data?.id || 0
    };

    const request$ = this.isEdit
      ? this.localesService.update(dto)
      : this.localesService.create(dto);

    request$.subscribe({
      next: () => {
        this.snackbar.open('Establecimiento guardado correctamente', 'Cerrar', { duration: 2000 });
        this.dialogRef?.close(true);
      },
      error: (err) => {
        this.snackbar.open('Error al guardar: ' + err.message, 'Cerrar');
      }
    });
  }

  cancel(): void {
    this.dialogRef?.close();
  }

}
