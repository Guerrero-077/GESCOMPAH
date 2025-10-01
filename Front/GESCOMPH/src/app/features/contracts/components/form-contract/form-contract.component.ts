import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors, ValidatorFn,
  Validators
} from '@angular/forms';

import { MatButtonModule } from '@angular/material/button';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

import { Subject, of } from 'rxjs';
import { catchError, distinctUntilChanged, finalize, map, switchMap, takeUntil } from 'rxjs/operators';

import { EstablishmentSelect } from '../../../establishments/models/establishment.models';
import { SquareSelectModel } from '../../../establishments/models/squares.models';
import { EstablishmentService } from '../../../establishments/services/establishment/establishment.service';
import { SquareService } from '../../../establishments/services/square/square.service';
import { PersonService } from '../../../security/services/person/person.service';
import { CitySelectModel } from '../../../setting/models/city.models';
import { CityService } from '../../../setting/services/city/city.service';
import { ClauseSelect } from '../../models/clause.models';
import { ContractCreateModel } from '../../models/contract.models';
import { ClauseService } from '../../services/clause/clause.service';
import { ContractService } from '../../services/contract/contract.service';

import { MatStepper } from "@angular/material/stepper";
import { ErrorMessageService } from '../../../../shared/Services/forms/error-message.service';
import { FormUtilsService, buildEmailValidators } from '../../../../shared/Services/forms/form-utils.service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { StandardButtonComponent } from '../../../../shared/components/standard-button/standard-button.component';
import { DocumentFormatDirective } from '../../../../shared/directives/document-format/document-format.directive';
import { MoneyPipe } from '../../../../shared/pipes/money.pipe';
import { AppValidators as AV } from '../../../../shared/utils/AppValidators';
import { FormErrorComponent } from '../../../../shared/components/form-error/form-error.component';


import { MatStepperModule } from '@angular/material/stepper';


/* ========== Validadores auxiliares ========== */

/**
 * Validador que asegura que la fecha de fin no sea anterior a la de inicio.
 *
 * @param startCtrl - Nombre del control de fecha de inicio.
 * @param endCtrl - Nombre del control de fecha de fin.
 * @returns `ValidatorFn` que devuelve `{ endBeforeStart: true }` si la fecha de fin es inválida, o `null` si es válido.
 */
function endAfterOrEqualStartValidator(startCtrl: string, endCtrl: string): ValidatorFn {
  return (group: AbstractControl): ValidationErrors | null => {
    const start = group.get(startCtrl)?.value as Date | null;
    const end = group.get(endCtrl)?.value as Date | null;
    if (!start || !end) return null;
    const normalize = (d: Date) => new Date(d.getFullYear(), d.getMonth(), d.getDate()).getTime();
    return normalize(end) < normalize(start) ? { endBeforeStart: true } : null;
  };
}

/**
 * Convierte un objeto Date en formato `YYYY-MM-DD`.
 *
 * @param d - Fecha a formatear.
 * @returns Cadena de texto representando la fecha.
 */
function toDateOnly(d: Date): string {
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
}

@Component({
  selector: 'app-form-contract',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatIconModule,
    StandardButtonComponent,
    DocumentFormatDirective,
    MatStepper,
    MatStepperModule,
    FormErrorComponent,
  ],
  templateUrl: './form-contract.component.html',
  styleUrls: ['./form-contract.component.css'],
})
export class FormContractComponent implements OnInit, OnDestroy {
  private readonly fb = inject(FormBuilder);
  private readonly citySvc = inject(CityService);
  private readonly estSvc = inject(EstablishmentService);
  private readonly contractSvc = inject(ContractService);
  private readonly personSvc = inject(PersonService);
  private readonly squareSvc = inject(SquareService);
  private readonly clauseSvc = inject(ClauseService);
  private readonly dialogRef = inject(MatDialogRef<FormContractComponent>);

  private readonly utils = inject(FormUtilsService);
  private readonly errMsg = inject(ErrorMessageService);
  private readonly sweet = inject(SweetAlertService);

  private readonly destroy$ = new Subject<void>();

  personFormGroup!: FormGroup;
  contractFormGroup!: FormGroup;
  establishmentFormGroup!: FormGroup;

  ciudades: CitySelectModel[] = [];
  plazas: SquareSelectModel[] = [];
  filteredEstablishments: EstablishmentSelect[] = [];
  clauses: ClauseSelect[] = [];

  personaEncontrada = false;
  personId: number | null = null;
  foundCityName: string | null = null;

  loadingPerson = false;
  loadingEstablishments = false;
  saving = false;

  today = new Date();
  minEndDate = this.today;
  startMinDate = new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDate());

  private lastQueriedDoc: string | null = null;
  private lastNotFoundAlertForDoc: string | null = null;

  // Mapas de mensajes para validadores personalizados
  docMessages = { colombianDocument: () => 'El documento debe tener entre 7 y 10 dígitos numéricos' } as const;
  firstNameMessages = { alphaHuman: () => 'Los nombres solo pueden contener letras y espacios' } as const;
  lastNameMessages = { alphaHuman: () => 'Los apellidos solo pueden contener letras y espacios' } as const;
  phoneMessages = { colombianPhone: () => 'Ingrese un número celular válido (debe empezar en 3 y tener 10 dígitos)' } as const;
  emailMessages = {
    domainMissing: () => 'El correo debe incluir un dominio (ej: @gmail.com)',
    tldMissing: () => 'El dominio debe incluir una extensión válida (.com, .co, etc.)'
  } as const;
  addressMessages = { addressInvalid: () => 'Ingrese una dirección válida' } as const;
  dateRangeMessages = { endBeforeStart: () => 'La fecha de finalización debe ser posterior o igual a la de inicio' } as const;

  /**
   * Hook de ciclo de vida de Angular.
   * Inicializa formularios, listas y observadores reactivos.
   */
  ngOnInit(): void {
    this.initForms();
    this.loadCiudades();
    this.loadPlazas();
    this.loadClauses();
    this.setupReactivePersonLookup();
    this.setupPlazaFiltering();
  }

  /**
   * Hook de ciclo de vida de Angular.
   * Cancela las suscripciones al destruir el componente.
   */
  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /* ===================== Forms ===================== */

  /**
   * Inicializa los formularios principales:
   * - PersonFormGroup: datos personales.
   * - ContractFormGroup: datos del contrato.
   * - EstablishmentFormGroup: relación con establecimientos.
   *
   * Configura validaciones personalizadas y dependencias entre campos.
   */
  private initForms(): void {
    this.personFormGroup = this.fb.group({
      document: this.fb.control(
        '',
        { validators: [Validators.required, AV.colombianDocument()], updateOn: 'blur' }
      ),
      firstName: ['', [Validators.required, AV.notOnlySpaces(), AV.alphaHumanName(), Validators.maxLength(50)]],
      lastName: ['', [Validators.required, AV.notOnlySpaces(), AV.alphaHumanName(), Validators.maxLength(50)]],
      phone: ['', [Validators.required, AV.colombianPhone()]],
      email: ['', buildEmailValidators(true)],
    });

    this.contractFormGroup = this.fb.group({
      address: ['', [Validators.required, AV.address()]],
      cityId: [null, Validators.required],
      startDate: [this.startMinDate, Validators.required],
      endDate: [this.startMinDate, Validators.required],
    }, { validators: endAfterOrEqualStartValidator('startDate', 'endDate') });

    this.establishmentFormGroup = this.fb.group({
      plazaId: [null, Validators.required],
      establishmentIds: [[], Validators.required],
      useSystemParameters: [true],
      clauseIds: [[]],
    });

    this.contractFormGroup.get('startDate')!
      .valueChanges.pipe(takeUntil(this.destroy$))
      .subscribe((d: Date) => {
        if (!d) return;
        const onlyDate = new Date(d.getFullYear(), d.getMonth(), d.getDate());
        this.minEndDate = onlyDate;
        const end = this.contractFormGroup.get('endDate')!.value as Date | null;
        if (end && end < onlyDate) this.contractFormGroup.get('endDate')!.setValue(onlyDate);
      });
  }

  /* ===================== Cargas ===================== */

  /**
   * Carga la lista de ciudades desde el servicio y la asigna a `ciudades`.
   */
  private loadCiudades(): void {
    this.citySvc.getAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe({ next: (res) => (this.ciudades = res ?? []) });
  }

  /**
   * Carga la lista de plazas desde el servicio y la asigna a `plazas`.
   */
  private loadPlazas(): void {
    this.squareSvc.getAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe({ next: (res) => (this.plazas = res ?? []) });
  }

  /**
   * Carga la lista de cláusulas desde el servicio y la asigna a `clauses`.
   * Filtra cláusulas sin ID válido.
   */
  private loadClauses(): void {
    this.clauseSvc.getAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe({ next: (res) => (this.clauses = (res ?? []).filter(c => c?.id != null)) });
  }

  /* ===================== Dependencias ===================== */

  /**
   * Configura la lógica para filtrar establecimientos al seleccionar una plaza.
   * Reinicia `establishmentIds` y consulta establecimientos activos.
   */
  private setupPlazaFiltering(): void {
    const estCtrl = this.establishmentFormGroup.get('establishmentIds')!;
    const plazaCtrl = this.establishmentFormGroup.get('plazaId')!;

    plazaCtrl.valueChanges.pipe(
      takeUntil(this.destroy$),
      map((id: number | string | null) => (id == null ? null : Number(id))),
      distinctUntilChanged(),
      switchMap((plazaId) => {
        estCtrl.setValue([], { emitEvent: false });
        estCtrl.disable({ emitEvent: false }); // 🔒 Deshabilita por defecto

        if (!plazaId || plazaId <= 0) return of([]);

        this.loadingEstablishments = true;
        return this.estSvc.getByPlaza(plazaId, { activeOnly: true }).pipe(
          catchError(() => of([])),
          finalize(() => {
            this.loadingEstablishments = false;

            // 🧠 Reactiva solo si hay plaza válida y terminó de cargar
            if (this.filteredEstablishments.length > 0) {
              estCtrl.enable({ emitEvent: false });
            }
          })
        );
      })
    ).subscribe((list) => {
      this.filteredEstablishments = list ?? [];

      // Habilita o no según el resultado, en caso de finalización muy rápida
      if (this.filteredEstablishments.length > 0) {
        estCtrl.enable({ emitEvent: false });
      } else {
        estCtrl.disable({ emitEvent: false });
      }
    });
  }


  /* ===================== Lookup persona ===================== */

  /**
   * Configura la búsqueda reactiva de personas por documento.
   * - Evita consultas si el documento es corto (< 5 caracteres).
   * - Actualiza los formularios si la persona existe.
   */
  private setupReactivePersonLookup(): void {
    this.personFormGroup.get('document')!
      .valueChanges.pipe(
        takeUntil(this.destroy$),
        map((v: string) => String(v ?? '').trim()),
        distinctUntilChanged(),
        switchMap((doc) => {
          if (!doc || doc.length < 5) {
            this.resetFoundPerson();
            return of(null);
          }
          this.loadingPerson = true;
          this.lastQueriedDoc = doc;
          this.lastNotFoundAlertForDoc = null; // reinicia alerta por nuevo documento consultado
          return this.personSvc.getByDocument(doc).pipe(
            // Si el backend responde 404, mostramos alerta (string o { detail })
            catchError((err) => {
              if (err?.status === 404) {
                const msg = typeof err?.error === 'string' ? err.error : (err?.error?.detail ?? 'La persona no existe.');
                this.sweet.showNotification('No encontrado', msg, 'warning');
                this.lastNotFoundAlertForDoc = this.lastQueriedDoc;
              }
              return of(null);
            }),
            finalize(() => (this.loadingPerson = false))
          );
        })
      )
      .subscribe((person: any | null) => {
        const currentDoc = String(this.personFormGroup.get('document')!.value ?? '').trim();
        if (this.lastQueriedDoc && currentDoc !== this.lastQueriedDoc) return;

        if (person) {
          this.setFoundPerson(person);
        } else {
          // Si no hubo alerta previa (p.ej. API devolvió null 200 OK), mostramos una por defecto
          if (currentDoc && currentDoc.length >= 5 && this.lastNotFoundAlertForDoc !== currentDoc) {
            this.sweet.showNotification('No encontrado', 'No existe una persona con el documento ingresado.', 'warning');
          }
          this.resetFoundPerson();
        }
      });
  }

  /* ===================== Helpers persona ===================== */

  /**
   * Establece la información de una persona encontrada en los formularios.
   *
   * @param p - Datos de la persona.
   */
  private setFoundPerson(p: any): void {
    this.personaEncontrada = true;
    this.personId = Number(p.id ?? null);
    this.foundCityName = p.cityName ?? null;
    this.personFormGroup.patchValue({
      firstName: p.firstName ?? '',
      lastName: p.lastName ?? '',
      phone: p.phone ?? '',
      email: p.email ?? '',
    }, { emitEvent: false });
    this.contractFormGroup.patchValue({
      address: p.address ?? '',
      cityId: p.cityId ?? null,
    }, { emitEvent: false });
    this.togglePersonFields(false);
  }

  /**
   * Resetea los formularios cuando no se encuentra la persona.
   * Habilita los campos para entrada manual.
   */
  private resetFoundPerson(): void {
    this.personaEncontrada = false;
    this.personId = null;
    this.foundCityName = null;
    this.personFormGroup.patchValue({ firstName: '', lastName: '', phone: '', email: '' }, { emitEvent: false });
    this.contractFormGroup.patchValue({ address: '', cityId: null }, { emitEvent: false });
    this.togglePersonFields(true);
  }

  /**
   * Habilita o deshabilita campos de persona y contrato.
   *
   * @param enable - `true` para habilitar campos, `false` para deshabilitarlos.
   */
  private togglePersonFields(enable: boolean): void {
    const personFields = ['firstName', 'lastName', 'phone', 'email'];
    const contractFields = ['address', 'cityId'];
    [...personFields, ...contractFields].forEach((k) =>
      enable
        ? this.personFormGroup.get(k)?.enable({ emitEvent: false })
        : this.personFormGroup.get(k)?.disable({ emitEvent: false })
    );
  }

  /* ===================== Acciones ===================== */

  /**
   * Cierra el diálogo sin guardar cambios.
   */
  cancel(): void {
    this.dialogRef.close(false);
  }

  /**
   * Envía el formulario para crear un contrato.
   * - Valida todos los formularios.
   * - Construye el payload con los valores normalizados.
   * - Llama al servicio de contratos y cierra el diálogo al terminar.
   */
  submit(): void {
    if (this.personFormGroup.invalid || this.contractFormGroup.invalid || this.establishmentFormGroup.invalid || this.saving) {
      this.markAllTouched();
      return;
    }

    const p = this.personFormGroup.getRawValue();
    const c = this.contractFormGroup.getRawValue();
    const est = this.establishmentFormGroup.getRawValue();

    const payload: ContractCreateModel = {
      startDate: toDateOnly(c.startDate),
      endDate: toDateOnly(c.endDate),
      address: String(c.address).trim(),
      cityId: Number(c.cityId),
      document: String(p.document).trim(),
      firstName: String(p.firstName).trim(),
      lastName: String(p.lastName).trim(),
      phone: String(p.phone).trim(),
      email: p.email ? String(p.email).trim() : null,
      establishmentIds: (est.establishmentIds as number[]).map(Number),
      useSystemParameters: !!est.useSystemParameters,
      clauseIds: Array.isArray(est.clauseIds) ? est.clauseIds.map(Number) : [],
    };

    this.saving = true;
    this.contractSvc.create(payload)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe({ next: () => this.dialogRef.close(true) });
  }

  /**
   * Corrige el dominio del email si el TLD es incorrecto.
   */
  fixEmail(): void {
    this.utils.coerceEmailTld(this.personFormGroup.get('email'));
  }

  /**
   * Marca todos los campos como tocados y actualiza su validez.
   * Fuerza la visualización de errores de validación.
   */
  markAllTouched(): void {
    [this.personFormGroup, this.contractFormGroup, this.establishmentFormGroup].forEach(g => {
      g.markAllAsTouched();
      Object.values(g.controls).forEach(c => c.updateValueAndValidity());
    });
  }

  /**
   * Obtiene el primer error de un control de formulario.
   *
   * @param control - Control del cual obtener el error.
   * @param order - Orden de prioridad de errores (opcional).
   * @returns Mensaje de error o `null` si no hay errores.
   */
  getFirstError(control: AbstractControl | null, order: string[] = []): string | null {
    return this.errMsg.firstError(control, order);
  }
}


