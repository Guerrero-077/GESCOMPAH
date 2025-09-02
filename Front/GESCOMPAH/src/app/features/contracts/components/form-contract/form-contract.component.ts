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
import { SquareService } from '../../../establishments/services/square/square.service';
import { SquareSelectModel } from '../../../establishments/models/squares.models';

// Validator and date functions (unchanged)
function endAfterOrEqualStartValidator(startCtrlName: string, endCtrlName: string): ValidatorFn {
  return (group: AbstractControl): ValidationErrors | null => {
    const start = group.get(startCtrlName)?.value as Date | null;
    const end = group.get(endCtrlName)?.value as Date | null;
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
    MatStepperModule // Added
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
  private readonly squareService = inject(SquareService); // Added
  private readonly dialogRef = inject(MatDialogRef<FormContractComponent>);

  private readonly destroy$ = new Subject<void>();

  // Form Groups for steps
  personFormGroup!: FormGroup;
  contractFormGroup!: FormGroup;
  establishmentFormGroup!: FormGroup;

  ciudades: CitySelectModel[] = [];
  plazas: SquareSelectModel[] = []; // Added
  allEstablishments: EstablishmentSelect[] = []; // Added
  filteredEstablishments: EstablishmentSelect[] = []; // Added

  personaEncontrada = false;
  personId: number | null = null;
  foundCityName: string | null = null;

  loadingPerson = false;
  saving = false;

  today = new Date();
  minEndDate = this.today;
  startMinDate = new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDate());
  startMaxDate = this.startMinDate;

  private lastQueriedDoc: string | null = null;

  ngOnInit(): void {
    this.initForms();
    this.loadCiudades();
    this.loadPlazas(); // Added
    this.loadAllEstablishments(); // Renamed
    this.setupReactivePersonLookup();
    this.setupPlazaFiltering(); // Added
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private initForms(): void {
    this.personFormGroup = this.fb.group({
      document: ['', [Validators.required, Validators.minLength(5)]],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      phone: ['', Validators.required],
      email: ['', Validators.email],
    });

    this.contractFormGroup = this.fb.group({
      address: ['', Validators.required],
      cityId: [null, Validators.required],
      startDate: [this.startMinDate, Validators.required],
      endDate: [this.startMinDate, Validators.required],
    }, { validators: endAfterOrEqualStartValidator('startDate', 'endDate') });

    this.establishmentFormGroup = this.fb.group({
      plazaId: [null, Validators.required],
      establishmentIds: [[], Validators.required],
      useSystemParameters: [true],
      clauseIds: [[]]
    });

    this.contractFormGroup.get('startDate')!.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe((d: Date) => {
        if (!d) return;
        const onlyDate = new Date(d.getFullYear(), d.getMonth(), d.getDate());
        this.minEndDate = onlyDate;
        const end = this.contractFormGroup.get('endDate')!.value as Date | null;
        if (end && end < onlyDate) this.contractFormGroup.get('endDate')!.setValue(onlyDate);
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

  private loadPlazas(): void {
    this.squareService.getAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => (this.plazas = res),
        error: err => console.error('Error al cargar plazas', err)
      });
  }

  private loadAllEstablishments(): void {
    this.establishmentService.getAllActive()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          this.allEstablishments = res;
          this.filteredEstablishments = []; // Initially empty until a plaza is selected
        },
        error: (err) => console.error('Error al cargar establecimientos', err)
      });
  }

  private setupPlazaFiltering(): void {
    this.establishmentFormGroup.get('plazaId')!.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(plazaId => {
        this.establishmentFormGroup.get('establishmentIds')!.setValue([], { emitEvent: false });
        if (plazaId) {
          this.filteredEstablishments = this.allEstablishments.filter(e => e.plazaId === plazaId);
        } else {
          this.filteredEstablishments = [];
        }
      });
  }

  private setupReactivePersonLookup(): void {
    this.personFormGroup.get('document')!.valueChanges.pipe(
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
      const currentDoc = (this.personFormGroup.get('document')!.value ?? '').trim();
      if (this.lastQueriedDoc && currentDoc !== this.lastQueriedDoc) return;

      if (person) {
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
    this.personFormGroup.patchValue({
      firstName: p.firstName ?? '',
      lastName:  p.lastName ?? '',
      phone:     p.phone ?? '',
      email:     p.email ?? '',
    }, { emitEvent: false });
    this.contractFormGroup.patchValue({
      address:   p.address ?? '',
      cityId:    p.cityId ?? null
    }, { emitEvent: false });
  }

  private resetFoundPersonState() {
    this.personaEncontrada = false;
    this.personId = null;
    this.foundCityName = null;
    this.personFormGroup.patchValue({
      firstName: '',
      lastName:  '',
      phone:     '',
      email:     '',
    }, { emitEvent: false });
    this.contractFormGroup.patchValue({
      address:   '',
      cityId:    null
    }, { emitEvent: false });
  }

  private disablePersonFields() {
    this.personFormGroup.get('firstName')?.disable({ emitEvent: false });
    this.personFormGroup.get('lastName')?.disable({ emitEvent: false });
    this.personFormGroup.get('phone')?.disable({ emitEvent: false });
    this.personFormGroup.get('email')?.disable({ emitEvent: false });
    this.contractFormGroup.get('address')?.disable({ emitEvent: false });
    this.contractFormGroup.get('cityId')?.disable({ emitEvent: false });
  }

  private enablePersonFields() {
    this.personFormGroup.get('firstName')?.enable({ emitEvent: false });
    this.personFormGroup.get('lastName')?.enable({ emitEvent: false });
    this.personFormGroup.get('phone')?.enable({ emitEvent: false });
    this.personFormGroup.get('email')?.enable({ emitEvent: false });
    this.contractFormGroup.get('address')?.enable({ emitEvent: false });
    this.contractFormGroup.get('cityId')?.enable({ emitEvent: false });
  }

  cancel(): void {
    this.dialogRef.close(false);
  }

  submit(): void {
    if (this.personFormGroup.invalid || this.contractFormGroup.invalid || this.establishmentFormGroup.invalid || this.saving) {
      return;
    }

    const personData = this.personFormGroup.getRawValue();
    const contractData = this.contractFormGroup.getRawValue();
    const establishmentData = this.establishmentFormGroup.getRawValue();

    const payload: ContractCreateModel = {
      startDate: toDateOnly(contractData.startDate),
      endDate:   toDateOnly(contractData.endDate),
      address: contractData.address,
      cityId: Number(contractData.cityId),
      document: personData.document,
      firstName: personData.firstName,
      lastName: personData.lastName,
      phone: personData.phone,
      email: personData.email || null,
      establishmentIds: establishmentData.establishmentIds,
      useSystemParameters: !!establishmentData.useSystemParameters,
      clauseIds: establishmentData.clauseIds || []
    };

    this.saving = true;
    this.contractService.createContract(payload).subscribe({
      next: _ => this.dialogRef.close(true),
      error: err => { console.error('❌ Error al crear contrato', err); this.saving = false; }
    });
  }
}
