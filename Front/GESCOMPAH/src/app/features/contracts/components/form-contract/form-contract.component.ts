import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import {
  ReactiveFormsModule, FormGroup, FormBuilder, Validators,
  AbstractControl, ValidationErrors, ValidatorFn
} from '@angular/forms';

import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatStepperModule } from '@angular/material/stepper';

import { Subject, of } from 'rxjs';
import { distinctUntilChanged, switchMap, tap, catchError, takeUntil, map, finalize } from 'rxjs/operators';

import { EstablishmentSelect } from '../../../establishments/models/establishment.models';
import { EstablishmentService } from '../../../establishments/services/establishment/establishment.service';
import { PersonService } from '../../../security/services/person/person.service';
import { CitySelectModel } from '../../../setting/models/city.models';
import { CityService } from '../../../setting/services/city/city.service';
import { ContractService } from '../../services/contract/contract.service';
import { ContractCreateModel } from '../../models/contract.models';
import { SquareService } from '../../../establishments/services/square/square.service';
import { SquareSelectModel } from '../../../establishments/models/squares.models';

import { AppValidators as AV } from '../../../../shared/utils/AppValidators';
import { buildEmailValidators, FormUtilsService } from '../../../../shared/Services/forms/form-utils.service';
import { ErrorMessageService } from '../../../../shared/Services/forms/error-message.service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';

/* ========== Validadores auxiliares ========== */
function endAfterOrEqualStartValidator(startCtrlName: string, endCtrlName: string): ValidatorFn {
  return (group: AbstractControl): ValidationErrors | null => {
    const start = group.get(startCtrlName)?.value as Date | null;
    const end   = group.get(endCtrlName)?.value as Date | null;
    if (!start || !end) return null;
    const y = (d: Date) => new Date(d.getFullYear(), d.getMonth(), d.getDate()).getTime();
    return y(end) < y(start) ? { endBeforeStart: true } : null;
  };
}
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
    MatSlideToggleModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatStepperModule,
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
  private readonly dialogRef = inject(MatDialogRef<FormContractComponent>);

  private readonly utils = inject(FormUtilsService);
  private readonly errMsg = inject(ErrorMessageService);
  private readonly sweetAlertService = inject(SweetAlertService);

  private readonly destroy$ = new Subject<void>();

  personFormGroup!: FormGroup;
  contractFormGroup!: FormGroup;
  establishmentFormGroup!: FormGroup;

  ciudades: CitySelectModel[] = [];
  plazas: SquareSelectModel[] = [];
  allEstablishments: EstablishmentSelect[] = [];
  filteredEstablishments: EstablishmentSelect[] = [];

  personaEncontrada = false;
  personId: number | null = null;
  foundCityName: string | null = null;
  loadingPerson = false;

  saving = false;

  today = new Date();
  minEndDate = this.today;
  startMinDate = new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDate());

  private lastQueriedDoc: string | null = null;

  ngOnInit(): void {
    this.initForms();
    this.loadCiudades();
    this.loadPlazas();
    this.loadAllEstablishments();
    this.setupReactivePersonLookup();
    this.setupPlazaFiltering();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /* ===================== Forms ===================== */
  private initForms(): void {
    this.personFormGroup = this.fb.group({
      // ⬇️ clave: updateOn: 'blur' para que valueChanges emita solo al perder foco
      document: this.fb.control(
        '',
        { validators: [Validators.required, AV.colombianDocument()], updateOn: 'blur' }
      ),
      firstName: ['', [Validators.required, AV.notOnlySpaces(), AV.alphaHumanName(), Validators.maxLength(50)]],
      lastName:  ['', [Validators.required, AV.notOnlySpaces(), AV.alphaHumanName(), Validators.maxLength(50)]],
      phone:     ['', [Validators.required, AV.colombianPhone()]],
      email:     ['', buildEmailValidators(true)],
    });

    this.contractFormGroup = this.fb.group({
      address:   ['', [Validators.required, AV.address()]],
      cityId:    [null, Validators.required],
      startDate: [this.startMinDate, Validators.required],
      endDate:   [this.startMinDate, Validators.required],
    }, { validators: endAfterOrEqualStartValidator('startDate', 'endDate') });

    this.establishmentFormGroup = this.fb.group({
      plazaId: [null, Validators.required],
      establishmentIds: [[], Validators.required],
      useSystemParameters: [true],
      clauseIds: [[]],
    });

    // Sincroniza fechas: endDate >= startDate
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

  /* ===================== Cargas iniciales ===================== */
  private loadCiudades(): void {
    this.citySvc.getAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => (this.ciudades = res ?? []),
        error: (err) => console.error('Error al cargar ciudades', err),
      });
  }

  private loadPlazas(): void {
    this.squareSvc.getAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => (this.plazas = res ?? []),
        error: (err) => console.error('Error al cargar plazas', err),
      });
  }

  private loadAllEstablishments(): void {
    this.estSvc.getAllActive()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          this.allEstablishments = res ?? [];
          this.filteredEstablishments = [];
        },
        error: (err) => console.error('Error al cargar establecimientos', err),
      });
  }

  /* ===================== Filtros dependientes ===================== */
  private setupPlazaFiltering(): void {
    this.establishmentFormGroup.get('plazaId')!
      .valueChanges.pipe(takeUntil(this.destroy$))
      .subscribe((plazaId: number | string | null) => {
        const id = plazaId == null ? null : Number(plazaId);
        this.establishmentFormGroup.get('establishmentIds')!.setValue([], { emitEvent: false });
        this.filteredEstablishments = id
          ? this.allEstablishments.filter((e) => Number(e.plazaId) === id)
          : [];
      });
  }

  /* ===================== Lookup persona (solo en blur) ===================== */
  private setupReactivePersonLookup(): void {
    this.personFormGroup.get('document')!
      .valueChanges.pipe(
        takeUntil(this.destroy$),
        map((v: string) => String(v ?? '').trim()),
        distinctUntilChanged(),
        tap((doc) => {
          if (!doc || doc.length < 5) {
            this.resetFoundPersonState();
            this.enablePersonFields();
          }
        }),
        switchMap((doc) => {
          if (!doc || doc.length < 5) return of(null); // corta llamadas si el doc es corto
          this.loadingPerson = true;
          this.lastQueriedDoc = doc;
          return this.personSvc.getByDocument(doc).pipe(
            catchError((err) => {
              if (err?.status === 404 && typeof err?.error === 'string') {
                this.sweetAlertService.toast(err.error, '', 'error');
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
          this.personaEncontrada = true;
          this.personId = Number(person.id ?? null);
          this.foundCityName = person.cityName ?? null;
          this.patchPerson(person);
          this.disablePersonFields();
        } else {
          this.resetFoundPersonState();
          this.enablePersonFields();
        }
      });
  }

  /* ===================== Helpers de patch/desbloqueo ===================== */
  private patchPerson(p: any): void {
    this.personFormGroup.patchValue(
      {
        firstName: p.firstName ?? '',
        lastName:  p.lastName ?? '',
        phone:     p.phone ?? '',
        email:     p.email ?? '',
      },
      { emitEvent: false }
    );
    this.contractFormGroup.patchValue(
      {
        address: p.address ?? '',
        cityId:  p.cityId ?? null,
      },
      { emitEvent: false }
    );
  }

  private resetFoundPersonState(): void {
    this.personaEncontrada = false;
    this.personId = null;
    this.foundCityName = null;
    this.personFormGroup.patchValue(
      { firstName: '', lastName: '', phone: '', email: '' },
      { emitEvent: false }
    );
    this.contractFormGroup.patchValue(
      { address: '', cityId: null },
      { emitEvent: false }
    );
  }

  private disablePersonFields(): void {
    ['firstName', 'lastName', 'phone', 'email'].forEach((k) =>
      this.personFormGroup.get(k)?.disable({ emitEvent: false })
    );
    ['address', 'cityId'].forEach((k) =>
      this.contractFormGroup.get(k)?.disable({ emitEvent: false })
    );
  }

  private enablePersonFields(): void {
    ['firstName', 'lastName', 'phone', 'email'].forEach((k) =>
      this.personFormGroup.get(k)?.enable({ emitEvent: false })
    );
    ['address', 'cityId'].forEach((k) =>
      this.contractFormGroup.get(k)?.enable({ emitEvent: false })
    );
  }

  /* ===================== Acciones ===================== */
  cancel(): void {
    this.dialogRef.close(false);
  }

  submit(): void {
    // Normalización/coerción antes de validar
    const p = this.personFormGroup;
    const c = this.contractFormGroup;

    const toNormalize: AbstractControl[] = [
      p.get('firstName')!, p.get('lastName')!, p.get('document')!,
      p.get('phone')!, c.get('address')!
    ];
    toNormalize.forEach(ctrl => this.utils.normalizeWhitespace(ctrl));
    this.utils.coerceEmailTld(p.get('email')); // auto .com si falta

    if (p.invalid || c.invalid || this.establishmentFormGroup.invalid || this.saving) {
      this.markAllTouched();
      return;
    }

    const person = p.getRawValue();
    const contract = c.getRawValue();
    const est = this.establishmentFormGroup.getRawValue();

    const payload: ContractCreateModel = {
      startDate: toDateOnly(contract.startDate),
      endDate:   toDateOnly(contract.endDate),
      address:   String(contract.address).trim(),
      cityId:    Number(contract.cityId),
      document:  String(person.document).trim(),
      firstName: String(person.firstName).trim(),
      lastName:  String(person.lastName).trim(),
      phone:     String(person.phone).trim(),
      email:     person.email ? String(person.email).trim() : null,
      establishmentIds: (est.establishmentIds as number[]).map(Number),
      useSystemParameters: !!est.useSystemParameters,
      clauseIds: Array.isArray(est.clauseIds) ? est.clauseIds.map(Number) : [],
    };

    this.saving = true;
    this.contractSvc.create(payload)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe({
        next: () => this.dialogRef.close(true),
        error: (err) => console.error('❌ Error al crear contrato', err),
      });
  }

  // Usado en (blur) del input email
  fixEmail(): void {
    this.utils.coerceEmailTld(this.personFormGroup.get('email'));
  }

  markAllTouched(): void {
    this.personFormGroup.markAllAsTouched();
    this.contractFormGroup.markAllAsTouched();
    this.establishmentFormGroup.markAllAsTouched();

    Object.values(this.personFormGroup.controls).forEach(c => c.updateValueAndValidity());
    Object.values(this.contractFormGroup.controls).forEach(c => c.updateValueAndValidity());
    Object.values(this.establishmentFormGroup.controls).forEach(c => c.updateValueAndValidity());
  }

  getFirstError(control: AbstractControl | null, order: string[] = []): string | null {
    return this.errMsg.firstError(control, order);
  }
}
