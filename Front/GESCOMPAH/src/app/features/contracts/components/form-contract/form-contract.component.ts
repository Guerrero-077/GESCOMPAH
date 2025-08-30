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
import Swal from 'sweetalert2';

import { Subject, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap, tap, catchError, takeUntil, map } from 'rxjs/operators';

import { EstablishmentSelect } from '../../../establishments/models/establishment.models';
import { EstablishmentService } from '../../../establishments/services/establishment/establishment.service';
import { PersonService } from '../../../security/services/person/person.service';
import { CitySelectModel } from '../../../setting/models/city.models';
import { CityService } from '../../../setting/services/city/city.service';
import { ContractService } from '../../services/contract/contract.service';
import { ContractCreateModel } from '../../models/contract.models';

// ---------- Validador: end >= start (normalizando a fecha sin hora) ----------
function endAfterOrEqualStartValidator(startCtrlName: string, endCtrlName: string): ValidatorFn {
  return (group: AbstractControl): ValidationErrors | null => {
    const start = group.get(startCtrlName)?.value as Date | null;
    const end = group.get(endCtrlName)?.value as Date | null;
    if (!start || !end) return null;
    const y = (d: Date) => new Date(d.getFullYear(), d.getMonth(), d.getDate()).getTime();
    return y(end) < y(start) ? { endBeforeStart: true } : null;
  };
}

// YYYY-MM-DD para evitar issues de zona horaria con .NET
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
    MatIconModule
  ],
  templateUrl: './form-contract.component.html',
  styleUrls: ['./form-contract.component.css']
})
export class FormContractComponent implements OnInit, OnDestroy {
  private readonly fb = inject(FormBuilder);
  private readonly ciudadService = inject(CityService);
  private readonly establishmentService = inject(EstablishmentService);
  private readonly contractService = inject(ContractService);
  private readonly personService = inject(PersonService);
  private readonly dialogRef = inject(MatDialogRef<FormContractComponent>);

  private readonly destroy$ = new Subject<void>();

  form!: FormGroup;
  ciudades: CitySelectModel[] = [];
  establecimientos: EstablishmentSelect[] = [];

  personaEncontrada = false;
  personId: number | null = null;
  foundCityName: string | null = null;

  loadingPerson = false;
  saving = false;

  today = new Date();
  minEndDate = this.today;

  // Para inicio: no deshabilitamos el control; lo dejamos readonly en HTML y min=max=hoy
  startMinDate = new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDate());
  startMaxDate = this.startMinDate;

  private lastQueriedDoc: string | null = null;

  ngOnInit(): void {
    this.initForm();
    this.loadCiudades();
    this.loadEstablecimientos();
    this.setupReactivePersonLookup();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private initForm(): void {
    const startOfToday = this.startMinDate;

    this.form = this.fb.group({
      // Inicio = HOY (enabled para que el icono abra calendar; lo hacemos readonly en HTML)
      startDate: [startOfToday, Validators.required],
      // Fin con mínimo = hoy
      endDate: [startOfToday, Validators.required],

      address: ['', Validators.required],
      cityId: [null, Validators.required],
      document: ['', [Validators.required, Validators.minLength(5)]],

      // Requeridos por el backend
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      phone: ['', Validators.required],

      // Backend: email opcional
      email: ['', Validators.email],

      establishmentIds: [[], Validators.required],

      useSystemParameters: [true],
      clauseIds: [[]]
    }, { validators: endAfterOrEqualStartValidator('startDate', 'endDate') });

    // Si cambia inicio (aunque no debería por min=max), mantenemos min de fin
    this.form.get('startDate')!.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe((d: Date) => {
        if (!d) return;
        const onlyDate = new Date(d.getFullYear(), d.getMonth(), d.getDate());
        this.minEndDate = onlyDate;
        const end = this.form.get('endDate')!.value as Date | null;
        if (end && end < onlyDate) this.form.get('endDate')!.setValue(onlyDate);
      });
  }

  private loadCiudades(): void {
    this.ciudadService.getAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => (this.ciudades = res),
        error: err => console.error('Error al cargar ciudades', err)
      });
  }

private loadEstablecimientos(): void {
  this.establishmentService.getAllActive()
    .subscribe({
      next: (res) => this.establecimientos = res,
      error: (err) => console.error('Error al cargar establecimientos', err)
    });
}

  // Búsqueda reactiva por documento
  private setupReactivePersonLookup(): void {
    this.form.get('document')!.valueChanges.pipe(
      takeUntil(this.destroy$),
      map((v: string) => (v ?? '').trim()),
      debounceTime(400),
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
        return this.personService.getByDocument(doc).pipe(
          catchError((err) => {
            if (err.status === 404 && typeof err.error === 'string') {
              Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'error',
                title: err.error,
                showConfirmButton: false,
                timer: 3500,
                timerProgressBar: true
              });
            }
            return of(null);
          }),
          tap(() => (this.loadingPerson = false))
        );
      })
    ).subscribe(person => {
      const currentDoc = (this.form.get('document')!.value ?? '').trim();
      if (this.lastQueriedDoc && currentDoc !== this.lastQueriedDoc) return;

      if (person) {
        // Esperado:
        // { firstName, lastName, document, address, phone, cityName, cityId, email, id }
        this.personaEncontrada = true;
        this.personId = person.id ?? null;
        this.foundCityName = person.cityName ?? null;
        this.patchPerson(person);
        this.disablePersonFields();
      } else {
        this.resetFoundPersonState();
        this.enablePersonFields();
      }
    });
  }

  private patchPerson(p: any) {
    this.form.patchValue({
      firstName: p.firstName ?? '',
      lastName:  p.lastName ?? '',
      phone:     p.phone ?? '',
      email:     p.email ?? '',
      address:   p.address ?? '',
      cityId:    p.cityId ?? null
    }, { emitEvent: false });
  }

  private resetFoundPersonState() {
    this.personaEncontrada = false;
    this.personId = null;
    this.foundCityName = null;
    this.form.patchValue({
      firstName: '',
      lastName:  '',
      phone:     '',
      email:     '',
      address:   '',
      cityId:    null
    }, { emitEvent: false });
  }

  // Bloquea los campos de persona encontrada (sin opacar — ver CSS .no-dim)
  private disablePersonFields() {
    this.form.get('firstName')?.disable({ emitEvent: false });
    this.form.get('lastName')?.disable({ emitEvent: false });
    this.form.get('phone')?.disable({ emitEvent: false });
    this.form.get('email')?.disable({ emitEvent: false });
    this.form.get('address')?.disable({ emitEvent: false });
    this.form.get('cityId')?.disable({ emitEvent: false });
  }

  private enablePersonFields() {
    this.form.get('firstName')?.enable({ emitEvent: false });
    this.form.get('lastName')?.enable({ emitEvent: false });
    this.form.get('phone')?.enable({ emitEvent: false });
    this.form.get('email')?.enable({ emitEvent: false });
    this.form.get('address')?.enable({ emitEvent: false });
    this.form.get('cityId')?.enable({ emitEvent: false });
  }

  cancel(): void {
    this.dialogRef.close(false);
  }

  submit(): void {
    if (this.form.invalid || this.saving) return;

    // getRawValue para incluir campos deshabilitados en payload
    const raw = this.form.getRawValue();

    const payload: ContractCreateModel = {
      startDate: toDateOnly(raw.startDate),
      endDate:   toDateOnly(raw.endDate),
      address: raw.address,
      cityId: Number(raw.cityId),
      document: raw.document,
      firstName: raw.firstName,
      lastName: raw.lastName,
      phone: raw.phone,
      email: raw.email || null,
      establishmentIds: raw.establishmentIds,
      useSystemParameters: !!raw.useSystemParameters,
      clauseIds: raw.clauseIds || []
    };

    this.saving = true;
    this.contractService.createContract(payload).subscribe({
      next: _ => this.dialogRef.close(true),
      error: err => { console.error('❌ Error al crear contrato', err); this.saving = false; }
    });
  }
}
