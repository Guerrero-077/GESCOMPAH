import { CommonModule } from '@angular/common';
import {
  Component, Inject, Optional, OnInit, OnDestroy, inject, ChangeDetectionStrategy
} from '@angular/core';
import {
  AbstractControl, FormControl, NonNullableFormBuilder, ReactiveFormsModule, Validators
} from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatStepperModule } from '@angular/material/stepper';
import { firstValueFrom } from 'rxjs';

import {
  EstablishmentCreate,
  EstablishmentSelect,
  EstablishmentUpdate,
  ImageSelectDto
} from '../../models/establishment.models';
import { EstablishmentStore } from '../../services/establishment/establishment.store';
import { EstablishmentService } from '../../services/establishment/establishment.service';
import { ImageService } from '../../services/image/image.service';
import { SquareService } from '../../services/square/square.service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { SquareSelectModel } from '../../models/squares.models';


// Soporte Drag & Drop aislado en directiva y servicio
import { FileDropDirective } from '../../../../shared/directives/file-drop.directive';
import { FilePickerService } from '../../../../shared/Services/Picker/file-picker.service';
import { GeneralForm, UbicacionForm } from '../../shapes/Formularios';

import { AppValidators } from '../../../../shared/utils/AppValidators';
import { StandardButtonComponent } from '../../../../shared/components/standard-button/standard-button.component';


/* Mensajes de error:
   - Declarados fuera de la clase para que NO se recreen por cada instancia.
   - readonly (as const) para inmutabilidad y mejor autocompletado. */
export const ERRORS = {
  name: {
    required: 'El nombre es obligatorio.',
    maxlength: 'El nombre no puede superar 100 caracteres.',
    onlySpaces: 'El nombre no puede ser solo espacios.'
  },
  description: {
    required: 'La descripción es obligatoria.',
    maxlength: 'La descripción no puede superar 500 caracteres.',
    onlySpaces: 'La descripción no puede ser solo espacios.'
  },
  rentValueBase: {
    required: 'El valor base es obligatorio.',
    NaN: 'Ingresa un número válido.',
    invalidNotation: 'No uses notación científica (e/E) ni signos +/−.',
    min: 'El valor base no puede ser menor que 1.',
    max: 'El valor base es demasiado alto.',
    decimalScale: 'Máximo 2 decimales permitidos.'
  },
  uvtQty: {
    required: 'La cantidad de UVT es obligatoria.',
    NaN: 'Ingresa un número válido.',
    invalidNotation: 'No uses notación científica (e/E) ni signos +/−.',
    min: 'La cantidad UVT no puede ser menor que 1.',
    max: 'La cantidad UVT es demasiado alta.',
    decimalScale: 'Máximo 2 decimales permitidos.'
  },
  areaM2: {
    required: 'El área es obligatoria.',
    NaN: 'Ingresa un número válido.',
    invalidNotation: 'No uses notación científica (e/E) ni signos +/−.',
    min: 'El área no puede ser menor que 1 m².',
    max: 'El área es demasiado alta.',
    decimalScale: 'Máximo 2 decimales permitidos.'
  },
  plazaId: {
    required: 'La plaza es obligatoria.'
  },
  address: {
    maxlength: 'La dirección no puede superar 150 caracteres.',
    addressInvalid: 'Usa solo letras, números, espacios y # - , .'
  }
} as const;


@Component({
  selector: 'app-form-establishment',
  standalone: true, // componente standalone: sin NgModule explícito
  imports: [
    CommonModule, ReactiveFormsModule, MatDialogModule, MatInputModule, MatButtonModule,
    MatSelectModule, MatIconModule, MatProgressSpinnerModule, MatTooltipModule,
    MatStepperModule,
    FileDropDirective, // directiva para manejar drag&drop de archivos
    StandardButtonComponent
  ],
  templateUrl: './form-establishment.component.html',
  styleUrls: ['./form-establishment.component.css'],
  // OnPush: sólo re-renderiza ante cambios de inputs/observables/refs → mejor rendimiento
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class FormEstablishmentComponent implements OnInit, OnDestroy {

  private readonly fb = inject(NonNullableFormBuilder);
  private readonly store = inject(EstablishmentStore);
  private readonly estSvc = inject(EstablishmentService);
  private readonly imageService = inject(ImageService);
  private readonly plazasSrv = inject(SquareService);
  private readonly dialogRef = inject(MatDialogRef<FormEstablishmentComponent, boolean>);
  private readonly sweetAlert = inject(SweetAlertService);
  private readonly dialog = inject(MatDialog);
  private readonly filePicker = inject(FilePickerService);

  /* ===== Estado UI simple ===== */
  isEdit = false;
  private _isBusy = false;
  get isBusy(): boolean { return this._isBusy; }
  set isBusy(v: boolean) {
    this._isBusy = v;
    // Deshabilita/habilita los formularios vía API de Reactive Forms
    // para evitar warnings de Angular sobre [disabled] en plantillas.
    const opts = { emitEvent: false } as const;
    if (v) {
      this.generalGroup.disable(opts);
      this.ubicacionGroup.disable(opts);
    } else {
      this.generalGroup.enable(opts);
      this.ubicacionGroup.enable(opts);
    }
  }
  isDeletingImage = false;

  /* ===== Formularios tipados ===== */

  readonly generalGroup = this.fb.group<GeneralForm>({
    name: this.fb.control('', {
      validators: [
        Validators.required,
        Validators.maxLength(100),
        AppValidators.notOnlySpaces()
      ]
    }),
    description: this.fb.control('', {
      validators: [
        Validators.required,
        Validators.maxLength(500),
        AppValidators.notOnlySpaces()
      ]
    }),
    rentValueBase: this.fb.control(0, {
      validators: [
        Validators.required,
        AppValidators.numberRange({ min: 1, max: 9_999_999.99 }),
        AppValidators.decimal({ decimals: 2 })
      ]
    }),
    uvtQty: this.fb.control(0, {
      validators: [
        Validators.required,
        AppValidators.numberRange({ min: 1, max: 9999 }),
        AppValidators.decimal({ decimals: 2 })
      ]
    }),
    areaM2: this.fb.control(0, {
      validators: [
        Validators.required,
        AppValidators.numberRange({ min: 1, max: 1_000_000 }),
        AppValidators.decimal({ decimals: 2 })
      ]
    }),
  }, { updateOn: 'blur' });

  readonly ubicacionGroup = this.fb.group<UbicacionForm>({
    plazaId: this.fb.control(0, {
      validators: [Validators.required]
    }),
    address: this.fb.control('', {
      validators: [
        Validators.maxLength(150),
        AppValidators.address()
      ]
    }),
  }, { updateOn: 'blur' });


  /* ===== Config de imágenes (UX + límites alineados con backend/Cloudinary) ===== */
  readonly MAX_IMAGES = 5;
  readonly MAX_FILE_SIZE_MB = 5;
  readonly MAX_FILE_SIZE_BYTES = this.MAX_FILE_SIZE_MB * 1024 * 1024;

  /* Colecciones separadas:
     - existingImagesFull: imágenes que ya existen en el backend (con id/publicId)
     - selectedFiles:     nuevos archivos seleccionados (File)
     - imagesPreview:     objectURLs para previsualizar los nuevos "File"
     - objectUrls:        lista para liberar memoria (revokeObjectURL en destroy) */
  readonly selectedFiles: File[] = [];
  readonly imagesPreview: string[] = [];
  private objectUrls: string[] = [];
  readonly existingImagesFull: ImageSelectDto[] = [];

  /* Catálogo de plazas para el select */
  Squares: SquareSelectModel[] = [];

  /* Mensajes de error disponibles en la plantilla */
  readonly errors = ERRORS;

  constructor(@Optional() @Inject(MAT_DIALOG_DATA) public data?: EstablishmentSelect) {
    // Detecta modo edición en cuanto llega data
    this.isEdit = !!data?.id;
  }

  /** Carga inicial: si es edición parchea valores y carga catálogo de plazas. */
  async ngOnInit(): Promise<void> {
    if (this.isEdit) this.patchValues();
    this.Squares = await firstValueFrom(this.plazasSrv.getAll());
  }

  /** Limpieza: revoca todos los objectURL para evitar fugas de memoria. */
  ngOnDestroy(): void {
    this.filePicker.revokeAll(this.objectUrls);
    this.objectUrls = [];
  }

  /** Rellena los form groups y cachea imágenes existentes desde `data`. */
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
    this.existingImagesFull.push(...(this.data.images ?? []));
  }

  /* =================== Normalizaciones de entrada =================== */


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
    const s = String(v).replace(',', '.');
    const n = Number(s);
    if (!Number.isNaN(n)) control.setValue(n, { emitEvent: false });
    control.updateValueAndValidity({ emitEvent: false });
  }

  /* =================== Gestión de archivos =================== */

  /**
   * Recibe archivos (sea desde la directiva de drop o desde input file),
   * valida formato/tamaño/límite y genera objectURLs para preview.
   */
  onFilesAdded(files: File[]) {
    const remaining = this.MAX_IMAGES - (this.selectedFiles.length + this.existingImagesFull.length);
    if (remaining <= 0) {
      this.sweetAlert.showNotification('Límite de imágenes', `Máximo ${this.MAX_IMAGES} imágenes permitidas`, 'warning');
      return;
    }

    // FilePicker centraliza lógica de aceptación/errores y crea objectURLs
    const { accepted, errors, objectUrls } = this.filePicker.pick(
      files,
      this.selectedFiles,
      { remaining, maxSizeBytes: this.MAX_FILE_SIZE_BYTES, acceptImagesOnly: true }
    );

    if (errors.length) this.sweetAlert.showNotification('Error en archivos', errors.join('\n'), 'error');

    // Actualiza colecciones de preview y caché de URLs para su posterior revoke
    this.imagesPreview.push(...objectUrls);
    this.objectUrls.push(...objectUrls);
    this.selectedFiles.push(...accepted);
  }

  /** Adaptador para `<input type="file">` → convierte FileList a File[] y delega. */
  handleFileInput(ev: Event) {
    const input = ev.target as HTMLInputElement;
    if (!input.files?.length) return;
    const arr: File[] = [];
    for (let i = 0; i < input.files.length; i++) {
      const f = input.files.item(i);
      if (f) arr.push(f);
    }
    this.onFilesAdded(arr);
  }

  /**
   * Elimina una imagen:
   * - Si es existente: llama API (idempotente) y la retira del array.
   * - Si es nueva: revoca objectURL y la saca de los arrays locales.
   */
  async removeImage(index: number, isExisting: boolean) {
    if (this.isDeletingImage) return;

    if (isExisting) {
      const image = this.existingImagesFull[index];
      this.isDeletingImage = true;
      try {
        await firstValueFrom(this.imageService.deleteImageById(image.id));
        this.existingImagesFull.splice(index, 1);
      } finally {
        this.isDeletingImage = false;
      }
    } else {
      const url = this.imagesPreview[index];
      if (url) {
        this.filePicker.revoke(url);
        this.objectUrls = this.objectUrls.filter(u => u !== url);
      }
      this.imagesPreview.splice(index, 1);
      this.selectedFiles.splice(index, 1);
    }
  }

  /* =================== Guardado (submit en 2 fases) ===================
     1) Crea o actualiza el establecimiento (JSON).
     2) Si hubo nuevas imágenes, las sube y sincroniza el store. */

  async onSubmit(): Promise<void> {
    if (this.isBusy) return;

    // Validación rápida: marca todo como touched para mostrar errores
    if (this.generalGroup.invalid || this.ubicacionGroup.invalid) {
      this.generalGroup.markAllAsTouched();
      this.ubicacionGroup.markAllAsTouched();
      this.sweetAlert.showNotification('Campos incompletos', 'Completa todos los campos requeridos', 'warning');
      return;
    }

    this.isBusy = true;
    this.dialogRef.disableClose = true;

    try {
      const dto = this.buildDto();
      const establishmentId = await this.createOrUpdateAndReturnId(dto);
      await this.uploadNewImagesIfAny(establishmentId);
      this.dialogRef.close(true);
    } catch (err: any) {
      this.sweetAlert.showNotification('Error', err?.error?.detail || 'No se pudo guardar el establecimiento.', 'error');
    } finally {
      this.isBusy = false;
      this.dialogRef.disableClose = false;
    }
  }

  /** Construye el DTO común para create/update desde los dos form groups. */
  private buildDto() {
    const base = this.generalGroup.getRawValue();
    const loc = this.ubicacionGroup.getRawValue();
    return {
      name: base.name,
      description: base.description,
      areaM2: base.areaM2,
      rentValueBase: base.rentValueBase,
      uvtQty: base.uvtQty,
      plazaId: loc.plazaId,
      address: loc.address
    };
  }

  /**
   * Ejecuta create o update según el modo:
   * - UPDATE: store.update y devuelve el id existente.
   * - CREATE: service.create (para obtener id del backend) + store.upsertOne.
   */
  private async createOrUpdateAndReturnId(dto: Omit<EstablishmentCreate, 'id'>): Promise<number> {
    if (this.isEdit && this.data) {
      await this.store.update({ id: this.data.id, ...dto } as EstablishmentUpdate);
      return this.data.id;
    } else {
      const created = await firstValueFrom(this.estSvc.create(dto as EstablishmentCreate));
      this.store.upsertOne(created);
      return created.id;
    }
  }

  /** Sube imágenes nuevas (si las hay) y sincroniza el store para refrescar la lista. */
  private async uploadNewImagesIfAny(establishmentId: number): Promise<void> {
    if (!this.selectedFiles.length) return;
    const uploaded = await firstValueFrom(this.imageService.uploadImages(establishmentId, this.selectedFiles));
    if (uploaded?.length) {
      this.existingImagesFull.push(...uploaded);
      this.store.applyImages(establishmentId, uploaded);
    }
  }

  /** Cierra el diálogo sin guardar. */
  cancel(): void { this.dialogRef.close(false); }

  /* =================== Helpers de preview =================== */

  /** Normaliza una entidad de imagen a su URL (soporta `filePath` y `FilePath`). */
  imgSrc(img: any): string {
    return img?.filePath ?? img?.FilePath ?? '';
  }

  /** Concatena fuentes de preview: primero existentes, luego nuevas. */
  private getAllPreviewSources(): string[] {
    const existing = (this.existingImagesFull ?? []).map((img) => this.imgSrc(img)).filter(Boolean);
    return [...existing, ...(this.imagesPreview ?? [])];
  }

  /** Abre el visor genérico con lista completa y posición inicial. */
  private openPreview(title: string, startIndex: number): void {
    import('../image-preview-dialog-component/image-preview-dialog-component.component').then(m => {
      this.dialog.open(m.ImagePreviewDialogComponent, {
        data: { title, imageList: this.getAllPreviewSources(), startIndex },
        panelClass: 'image-preview-dialog',
        maxWidth: '95vw',
        width: 'auto',
        autoFocus: false
      });
    });
  }

  /** Preview arrancando en una imagen existente. */
  openPreviewFromExisting(index: number): void {
    if (index < 0 || index >= this.existingImagesFull.length) return;
    this.openPreview(`Imagen existente (${index + 1})`, index);
  }

  /** Preview arrancando en una imagen recién seleccionada. */
  openPreviewFromNew(index: number): void {
    if (index < 0 || index >= this.imagesPreview.length) return;
    const offsetExisting = this.existingImagesFull?.length ?? 0;
    this.openPreview(`Imagen nueva (${index + 1})`, offsetExisting + index);
  }

  /** trackBy para *ngFor: evita rerenderizar todas las tarjetas al cambiar una. */
  trackByIndex(index: number): number {
    return index;
  }
}
