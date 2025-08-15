import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
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
import { BehaviorSubject } from 'rxjs';

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
  // catálogos
  departments$ = this.deptStore.departments$;
  private _cities = new BehaviorSubject<CitySelectModel[]>([]);
  cities$ = this._cities.asObservable();

  // estado UI
  submitting = signal(false);
  submitError = signal<string | null>(null);

  // ====== FORM ROOT ======
  form: FormGroup = this.fb.group({
    // Paso 1: Ubicación (solo ids aquí)
    location: this.fb.group({
      departmentId: [null, Validators.required],
      cityId: [null, Validators.required],
    }),

    // Paso 2: Persona (campos que el backend exige como obligatorios)
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

  // ===== GETTERS =====
  get locationGroup(): FormGroup { return this.form.get('location') as FormGroup; }
  get personGroup(): FormGroup { return this.form.get('person') as FormGroup; }
  get accountGroup(): FormGroup { return this.form.get('account') as FormGroup; }

  // atajos de controles
  get email() { return this.accountGroup.get('email')!; }
  get password() { return this.accountGroup.get('password')!; }

  ngOnInit(): void {
    this.locationGroup.get('departmentId')!.valueChanges.subscribe((deptId: number | null) => {
      this.locationGroup.get('cityId')!.reset();
      if (deptId) {
        this.cityService.getCitiesByDepartment(deptId).subscribe({
          next: cities => this._cities.next(cities),
          error: () => this._cities.next([])
        });
      } else {
        this._cities.next([]);
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

    // normalizar email a minúsculas
    const emailLower = String(this.email.value ?? '').trim().toLowerCase();
    this.email.setValue(emailLower, { emitEvent: false });

    const loc = this.locationGroup.value;
    const p = this.personGroup.value;
    const acc = this.accountGroup.value;

    // ===== Payload PLANO que espera tu backend =====
    const payload = {
      firstName: p.firstName,
      lastName: p.lastName,
      document: p.document,
      phone: p.phone,
      address: p.address,
      email: emailLower,
      password: acc.password,
      cityId: Number(loc.cityId)
    };

    this.auth.Register(payload as any).subscribe({
      next: () => {
        this.submitting.set(false);
        this.form.reset();},
      complete: () => {
        this.router.navigate(['/auth/login']);
        this.submitting.set(false);
        this.form.reset();
      },
      error: (err) => {
        this.submitting.set(false);
        // Intenta leer modelState del backend y mostrar mensaje amigable
        const msg =
          err?.error?.title ||
          err?.error?.message ||
          'No se pudo registrar el usuario';
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

