import { CommonModule } from '@angular/common';
import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import {
  ReactiveFormsModule, FormGroup, FormBuilder, Validators,
  AbstractControl, ValidatorFn, ValidationErrors
} from '@angular/forms';

import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { MatStepperModule } from '@angular/material/stepper';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { StandardButtonComponent } from '../../../../shared/components/standard-button/standard-button.component';

import { Subject, of } from 'rxjs';
import { distinctUntilChanged, switchMap, tap, catchError, takeUntil, map, finalize } from 'rxjs/operators';

import { CityService } from '../../../setting/services/city/city.service';
import { PersonService } from '../../../security/services/person/person.service';
import { AppointmentService } from '../../services/appointment/appointment.service';

import { AppValidators as AV } from '../../../../shared/utils/AppValidators';
import { buildEmailValidators, FormUtilsService } from '../../../../shared/Services/forms/form-utils.service';
import { ErrorMessageService } from '../../../../shared/Services/forms/error-message.service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { AppointmentCreateModel } from '../../models/appointment.models';

function toDateOnly(d: Date): string {
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2,'0')}-${String(d.getDate()).padStart(2,'0')}`;
}

@Component({
  selector: 'app-form-appointment',
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
    MatStepperModule,
    MatProgressSpinnerModule,
    StandardButtonComponent
  ],
  templateUrl: './form-appointment.component.html',
  styleUrls: ['./form-appointment.component.css']
})
export class FormAppointmentComponent implements OnInit, OnDestroy {
  private readonly fb = inject(FormBuilder);
  private readonly citySvc = inject(CityService);
  private readonly personSvc = inject(PersonService);
  private readonly appointmentSvc = inject(AppointmentService);
  private readonly dialogRef = inject(MatDialogRef<FormAppointmentComponent>);
  private readonly utils = inject(FormUtilsService);
  private readonly errMsg = inject(ErrorMessageService);
  private readonly sweet = inject(SweetAlertService);

  private readonly destroy$ = new Subject<void>();

  personFormGroup!: FormGroup;
  appointmentFormGroup!: FormGroup;

  ciudades: any[] = [];
  personaEncontrada = false;
  personId: number | null = null;
  foundCityName: string | null = null;
  loadingPerson = false;
  saving = false;

  today = new Date();

  private lastQueriedDoc: string | null = null;

  ngOnInit(): void {
    this.initForms();
    this.loadCiudades();
    this.setupReactivePersonLookup();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /* ===================== Forms ===================== */
  private initForms(): void {
    this.personFormGroup = this.fb.group({
      document: this.fb.control('', { validators: [Validators.required, AV.colombianDocument()], updateOn: 'blur' }),
      firstName: ['', [Validators.required, AV.notOnlySpaces(), AV.alphaHumanName(), Validators.maxLength(50)]],
      lastName: ['', [Validators.required, AV.notOnlySpaces(), AV.alphaHumanName(), Validators.maxLength(50)]],
      phone: ['', [Validators.required, AV.colombianPhone()]],
      email: ['', buildEmailValidators(true)],
    });

    this.appointmentFormGroup = this.fb.group({
      description: ['', Validators.required],
      observation: [''],
      requestDate: [this.today, Validators.required],
      dateTimeAssigned: [this.today, Validators.required],
      cityId: [null, Validators.required],
      establishmentId: [null, Validators.required]
    });
  }

  /* ===================== Loads ===================== */
  private loadCiudades(): void {
    this.citySvc.getAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => (this.ciudades = res ?? []),
        error: (err) => console.error('Error al cargar ciudades', err)
      });
  }

  /* ===================== Lookup persona ===================== */
  private setupReactivePersonLookup(): void {
    this.personFormGroup.get('document')!
      .valueChanges.pipe(
        takeUntil(this.destroy$),
        map(v => String(v ?? '').trim()),
        distinctUntilChanged(),
        tap(doc => {
          if (!doc || doc.length < 5) {
            this.resetFoundPersonState();
            this.enablePersonFields();
          }
        }),
        switchMap(doc => {
          if (!doc || doc.length < 5) return of(null);
          this.loadingPerson = true;
          this.lastQueriedDoc = doc;
          return this.personSvc.getByDocument(doc).pipe(
            catchError(err => {
              if (err?.status === 404 && typeof err?.error === 'string') {
                this.sweet.showNotification('No encontrado', err.error, 'error');
              }
              return of(null);
            }),
            finalize(() => (this.loadingPerson = false))
          );
        })
      )
      .subscribe(person => {
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

  private patchPerson(p: any): void {
    this.personFormGroup.patchValue(
      {
        firstName: p.firstName ?? '',
        lastName: p.lastName ?? '',
        phone: p.phone ?? '',
        email: p.email ?? ''
      },
      { emitEvent: false }
    );

    this.appointmentFormGroup.patchValue(
      { cityId: p.cityId ?? null },
      { emitEvent: false }
    );
  }

  private resetFoundPersonState(): void {
    this.personaEncontrada = false;
    this.personId = null;
    this.foundCityName = null;
    this.personFormGroup.patchValue({ firstName: '', lastName: '', phone: '', email: '' }, { emitEvent: false });
    this.appointmentFormGroup.patchValue({ cityId: null }, { emitEvent: false });
  }

  private disablePersonFields(): void {
    ['firstName', 'lastName', 'phone', 'email'].forEach(k => this.personFormGroup.get(k)?.disable({ emitEvent: false }));
    ['cityId'].forEach(k => this.appointmentFormGroup.get(k)?.disable({ emitEvent: false }));
  }

  private enablePersonFields(): void {
    ['firstName', 'lastName', 'phone', 'email'].forEach(k => this.personFormGroup.get(k)?.enable({ emitEvent: false }));
    ['cityId'].forEach(k => this.appointmentFormGroup.get(k)?.enable({ emitEvent: false }));
  }

  /* ===================== Acciones ===================== */
  cancel(): void {
    this.dialogRef.close(false);
  }

  submit(): void {
    const p = this.personFormGroup;
    const a = this.appointmentFormGroup;

    this.utils.normalizeWhitespace(p.get('firstName'));
    this.utils.normalizeWhitespace(p.get('lastName'));
    this.utils.normalizeWhitespace(p.get('document'));
    this.utils.normalizeWhitespace(p.get('phone'));
    this.utils.coerceEmailTld(p.get('email'));

    if (p.invalid || a.invalid || this.saving) {
      p.markAllAsTouched();
      a.markAllAsTouched();
      return;
    }

    const payload: AppointmentCreateModel = {
      firstName: String(p.get('firstName')!.value).trim(),
      lastName: String(p.get('lastName')!.value).trim(),
      document: String(p.get('document')!.value).trim(),
      adrress: String(a.get('address')?.value ?? '').trim(),
      phone: String(p.get('phone')!.value).trim(),
      cityId: Number(a.get('cityId')!.value),
      establishmentId: Number(a.get('establishmentId')!.value),
      description: String(a.get('description')!.value).trim(),
      observation: String(a.get('observation')!.value ?? '').trim(),
      requestDate: a.get('requestDate')!.value,
      dateTimeAssigned: a.get('dateTimeAssigned')!.value
    };

    this.saving = true;
    this.appointmentSvc.create(payload)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe({
        next: () => this.dialogRef.close(true),
        error: err => console.error('❌ Error al crear cita', err)
      });
  }

  fixEmail(): void {
    this.utils.coerceEmailTld(this.personFormGroup.get('email'));
  }

  getFirstError(control: AbstractControl | null, order: string[] = []): string | null {
    return this.errMsg.firstError(control, order);
  }
}
