import { CommonModule } from '@angular/common';
import { Component, OnInit, DestroyRef, inject, signal } from '@angular/core';
import {
  AbstractControl,
  FormBuilder, FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  Validators
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatStepperModule } from '@angular/material/stepper';
import { BehaviorSubject, catchError, distinctUntilChanged, finalize, of, switchMap, tap, map } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { MatIconModule } from '@angular/material/icon';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../../core/service/auth/auth.service';
import { CitySelectModel } from '../../../setting/models/city.models';
import { CityService } from '../../../setting/services/city/city.service';
import { DepartmentStore } from '../../../setting/services/department/department.store';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatStepperModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  // inyección de dependencias
  private fb = inject(FormBuilder);
  private deptStore = inject(DepartmentStore);
  private cityService = inject(CityService);
  private auth = inject(AuthService);
  private router = inject(Router);
  private destroyRef = inject(DestroyRef);

  // catálogos
  departments$ = this.deptStore.departments$;
  private _cities = new BehaviorSubject<CitySelectModel[]>([]);
  cities$ = this._cities.asObservable();

  // estado UI
  submitting = signal(false);
  submitError = signal<string | null>(null);
  loadingCities = signal(false);

  // ====== FORM ROOT ======
  form: FormGroup = this.fb.group({
    // Paso 1: Ubicación
    location: this.fb.group({
      departmentId: [null, Validators.required],
      // deshabilitado solo si NO hay departamento
      cityId: [{ value: null, disabled: true }, [Validators.required, Validators.min(1)]],
    }),

    // Paso 2: Persona
    person: this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.maxLength(50)]],
      document: ['', Validators.required],
      phone: ['', Validators.required],
      address: ['', Validators.required],
    }),

    // Paso 3: Cuenta
    account: this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [
        Validators.required, Validators.minLength(8),
        hasUpper(), hasLower(), hasDigit(), hasSymbol()
      ]],
      confirmPassword: ['', Validators.required]
    }, { validators: passwordsMatch('password', 'confirmPassword') }),
  });

  // GETTERS
  get locationGroup(): FormGroup { return this.form.get('location') as FormGroup; }
  get personGroup(): FormGroup { return this.form.get('person') as FormGroup; }
  get accountGroup(): FormGroup { return this.form.get('account') as FormGroup; }
  get email() { return this.accountGroup.get('email')!; }

  // comparación tolerante a tipos para mat-select
  compareById = (a: any, b: any) => {
    if (a == null || b == null) return false;
    return String(a) === String(b);
  };

  ngOnInit(): void {
    const deptCtrl = this.locationGroup.get('departmentId')!;
    const cityCtrl = this.locationGroup.get('cityId')!;

    // Reaccionar al departamento
    deptCtrl.valueChanges.pipe(
      takeUntilDestroyed(this.destroyRef),
      distinctUntilChanged(),
      tap((deptId) => {
        // reset y estado de carga
        cityCtrl.reset({ value: null, disabled: !deptId }, { emitEvent: false });
        // si hay dept, el control debe quedar habilitado SIEMPRE (aunque no haya ciudades)
        if (deptId) cityCtrl.enable({ emitEvent: false }); else cityCtrl.disable({ emitEvent: false });
        this._cities.next([]);
        this.loadingCities.set(!!deptId);
      }),
      switchMap((deptId: number | string | null) => {
        const id = deptId == null ? null : Number(deptId);
        if (!id || Number.isNaN(id)) {
          return of([]).pipe(finalize(() => this.loadingCities.set(false)));
        }
        return this.cityService.getCitiesByDepartment(id).pipe(
          map((res: any) => {
            const raw = Array.isArray(res) ? res
              : Array.isArray(res?.data) ? res.data
                : Array.isArray(res?.items) ? res.items
                  : Array.isArray(res?.result) ? res.result
                    : [];
            return raw.map((c: any) => ({
              id: Number(c.id ?? c.cityId ?? c.value),
              name: String(c.name ?? c.cityName ?? c.label ?? '').trim(),
            })).filter((c: any) => !!c.id && !!c.name);
          }),
          catchError(() => of([])),
          finalize(() => this.loadingCities.set(false))
        );
      })
    ).subscribe((list: CitySelectModel[]) => {
      // Actualiza catálogo
      this._cities.next(list);

      // Importante:
      // - si HAY dept: cityId debe estar habilitado SIEMPRE
      // - si NO hay ciudades: forzamos error "required" para bloquear el paso
      const cityCtrl = this.locationGroup.get('cityId')!;
      if (this.locationGroup.get('departmentId')?.value) {
        cityCtrl.enable({ emitEvent: false });
        if (list.length === 0) {
          cityCtrl.setErrors({ required: true });
          cityCtrl.markAsTouched();
        } else {
          // si hay ciudades y el valor actual no es válido, dejamos el required normal actuar
          if (cityCtrl.value == null || Number(cityCtrl.value) < 1) {
            cityCtrl.setErrors({ required: true });
          } else {
            // limpia errores si seleccionó algo válido
            if (cityCtrl.valid) cityCtrl.setErrors(null);
          }
        }
      } else {
        cityCtrl.disable({ emitEvent: false });
      }
    });
  }

  submit() {
    this.submitError.set(null);
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.submitting.set(true);

    const emailLower = String(this.email.value ?? '').trim().toLowerCase();
    this.email.setValue(emailLower, { emitEvent: false });

    const loc = this.locationGroup.value;
    const p = this.personGroup.value;
    const acc = this.accountGroup.value;

    const payload = {
      firstName: p.firstName,
      lastName: p.lastName,
      document: p.document,
      phone: p.phone,
      address: p.address,
      email: emailLower,
      password: acc.password,
      cityId: Number(loc.cityId ?? 0)
    };

    this.auth.Register(payload as any).subscribe({
      next: () => {
        this.router.navigate(['/auth/login']);
        this.form.reset();
        this.submitting.set(false);
      },
      error: (err) => {
        this.submitting.set(false);
        const msg = err?.error?.title || err?.error?.message || 'No se pudo registrar el usuario';
        this.submitError.set(msg);
      }
    });
  }
}

/* ====== VALIDADORES REUTILIZABLES ====== */
function passwordsMatch(passKey: string, confirmKey: string) {
  return (group: AbstractControl): ValidationErrors | null => {
    const pass = group.get(passKey)?.value;
    const conf = group.get(confirmKey)?.value;
    return pass && conf && pass !== conf ? { passwordsMismatch: true } : null;
  };
}
function hasUpper() { return (c: AbstractControl) => /[A-Z]/.test(c.value || '') ? null : { upper: true }; }
function hasLower() { return (c: AbstractControl) => /[a-z]/.test(c.value || '') ? null : { lower: true }; }
function hasDigit() { return (c: AbstractControl) => /\d/.test(c.value || '') ? null : { digit: true }; }
function hasSymbol() { return (c: AbstractControl) => /[\W_]/.test(c.value || '') ? null : { symbol: true }; }
