import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors, FormGroup } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../../core/service/auth/auth.service';
import { CommonModule } from '@angular/common';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';

function notOnlySpaces(): (control: AbstractControl) => ValidationErrors | null {
  return (control: AbstractControl) => {
    const v = control.value as string;
    if (v == null) return null;
    return v.trim().length === 0 ? { onlySpaces: true } : null;
  };
}

/**
 * Regex que obliga:
 * - 1–254 chars totales
 * - 1–64 chars antes de @
 * - dominio con al menos un punto y TLD de 2+ letras
 */
const FULL_EMAIL_REGEX =
  /^(?=.{1,254}$)(?=.{1,64}@)[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i;

/** Devuelve true si hay @ pero no hay punto en la parte del dominio */
function hasAtButNoTld(email: string): boolean {
  const idx = email.indexOf('@');
  if (idx === -1) return false;
  const domain = email.slice(idx + 1);
  return domain.length > 0 && !domain.includes('.');
}

/** Si el correo tiene @ pero no TLD, agrega ".com" */
function appendDefaultTldIfNeeded(email: string, defaultTld = '.com'): string {
  const trimmed = (email ?? '').trim();
  if (!trimmed) return trimmed;
  if (hasAtButNoTld(trimmed)) {
    return trimmed + defaultTld;
  }
  return trimmed;
}

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    CommonModule,
    RouterModule
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);
  private sweetAlertService = inject(SweetAlertService);

  passwordVisible = false;

  readonly errorMessages = {
    email: {
      required: 'El correo es obligatorio.',
      email: 'Ingresa un correo válido (ej. usuario&#64;dominio.com).',
      maxlength: 'El correo no puede superar 254 caracteres.',
      onlySpaces: 'El correo no puede ser solo espacios.',
      pattern: 'Ingresa un correo completo con dominio y TLD (ej. usuario&#64;dominio.com).'
    },
    password: {
      required: 'La contraseña es obligatoria.',
      minlength: 'La contraseña debe tener al menos 6 caracteres.',
      maxlength: 'La contraseña no puede superar 128 caracteres.',
      onlySpaces: 'La contraseña no puede ser solo espacios.'
    }
  } as const;

  formLogin: FormGroup = this.fb.nonNullable.group({
    email: this.fb.nonNullable.control<string>('', {
      validators: [
        Validators.required,
        Validators.email,          // chequeo básico
        Validators.maxLength(254),
        notOnlySpaces(),
        Validators.pattern(FULL_EMAIL_REGEX) // exige TLD
      ],
      updateOn: 'blur'
    }),
    password: this.fb.nonNullable.control<string>('', {
      validators: [
        Validators.required,
        Validators.minLength(6),
        Validators.maxLength(128),
        notOnlySpaces()
      ],
      updateOn: 'change'
    })
  });

  // Accesores
  get emailCtrl() { return this.formLogin.get('email')!; }
  get passwordCtrl() { return this.formLogin.get('password')!; }

  isInvalid(control: AbstractControl | null): boolean {
    return !!control && control.invalid && (control.dirty || control.touched);
  }

  firstErrorOf(controlName: 'email' | 'password'): string | null {
    const ctrl = this.formLogin.get(controlName);
    if (!ctrl || !ctrl.errors) return null;
    const map = this.errorMessages[controlName];
    for (const key of Object.keys(ctrl.errors)) {
      if ((map as any)[key]) return (map as any)[key];
    }
    return 'Valor inválido.';
  }

  constructor() { }

  ngOnInit(): void { }

  togglePasswordVisibility(): void {
    this.passwordVisible = !this.passwordVisible;
  }

  me() {
    this.auth.GetMe().subscribe({
      next: () => { },
      error: (err) => {
        this.sweetAlertService.error(
          err?.message ?? 'Error inesperado',
          'Oops...'
        );
      }
    });
  }

  /** Hook del blur del email: agrega .com si falta TLD y normaliza */
  onEmailBlur() {
    const current = (this.emailCtrl.value ?? '').toLowerCase().trim();
    const patched = appendDefaultTldIfNeeded(current, '.com');
    if (patched !== current) {
      this.emailCtrl.setValue(patched, { emitEvent: false });
      this.emailCtrl.updateValueAndValidity();
    }
  }

  login() {
    if (this.formLogin.invalid) {
      this.formLogin.markAllAsTouched();
      // Intento adicional: si solo falta TLD, parchear y revalidar
      const fixed = appendDefaultTldIfNeeded((this.emailCtrl.value ?? '').toLowerCase().trim(), '.com');
      if (fixed !== this.emailCtrl.value) {
        this.emailCtrl.setValue(fixed, { emitEvent: false });
        this.emailCtrl.updateValueAndValidity();
      }
      if (this.formLogin.invalid) return;
    }

    // Normaliza antes de enviar (lower + trim + .com si falta)
    let email = (this.emailCtrl.value ?? '').toLowerCase().trim();
    email = appendDefaultTldIfNeeded(email, '.com');

    const password = this.passwordCtrl.value ?? '';

    this.sweetAlertService.showLoading('Iniciando sesión', 'Por favor, espere...');

    this.auth.Login({ email, password }).subscribe({
      next: () => {
        this.sweetAlertService.hideLoading();
        this.router.navigate(['/admin/dashboard']);
      },
      error: (err) => {
        this.sweetAlertService.hideLoading();
        this.sweetAlertService.toast(
          'Oops...',
          err?.error?.message ?? 'Credenciales inválidas',
          'error'
        );
      }
    });
  }
}
