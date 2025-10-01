import { Component, inject, OnInit } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
  AbstractControl,
  ValidationErrors,
  FormGroup,
} from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../../core/services/auth/auth.service';
import { CommonModule } from '@angular/common';
// Ionic standalone
import {
  IonAlert,
  IonButton,
  IonLoading,
  IonContent,
  IonIcon,
  IonInput,
  IonItem,
  IonText,
  IonGrid,
  IonRow,
  IonCol,
  IonImg,
  IonLabel,
  IonNote,
  IonCard,
  IonCardHeader,
  IonCardTitle,
  IonCardSubtitle,
  IonCardContent
} from '@ionic/angular/standalone';

function notOnlySpaces(): (
  control: AbstractControl
) => ValidationErrors | null {
  return (control: AbstractControl) => {
    const v = control.value as string;
    if (v == null) return null;
    return v.trim().length === 0 ? { onlySpaces: true } : null;
  };
}

const FULL_EMAIL_REGEX =
  /^(?=.{1,254}$)(?=.{1,64}@)[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i;

function hasAtButNoTld(email: string): boolean {
  const idx = email.indexOf('@');
  if (idx === -1) return false;
  const domain = email.slice(idx + 1);
  return domain.length > 0 && !domain.includes('.');
}

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
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  imports: [
    IonNote, IonLabel, IonCardContent, IonCardSubtitle, IonCardTitle, IonCardHeader, IonCard,
    IonImg, IonCol, IonRow, IonGrid,
    CommonModule, ReactiveFormsModule, RouterModule,
    IonContent, IonLoading, IonAlert, IonButton, IonItem, IonInput, IonText, IonIcon,
  ],
})
export class LoginComponent implements OnInit {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  passwordVisible = false;

  // estado del alert
  showAlert = false;
  alertHeader = '';
  alertMessage = '';
  alertButtons = ['OK'];

  readonly errorMessages = {
    email: {
      required: 'El correo es obligatorio.',
      email: 'Ingresa un correo válido (ej. usuario&#64;dominio.com).',
      maxlength: 'El correo no puede superar 254 caracteres.',
      onlySpaces: 'El correo no puede ser solo espacios.',
      pattern: 'Ingresa un correo completo con dominio y TLD (ej. usuario&#64;dominio.com).',
    },
    password: {
      required: 'La contraseña es obligatoria.',
      minlength: 'La contraseña debe tener al menos 6 caracteres.',
      maxlength: 'La contraseña no puede superar 128 caracteres.',
      onlySpaces: 'La contraseña no puede ser solo espacios.',
    },
  } as const;

  formLogin: FormGroup = this.fb.nonNullable.group({
    email: this.fb.nonNullable.control<string>('', {
      validators: [
        Validators.required,
        Validators.email,
        Validators.maxLength(254),
        notOnlySpaces(),
        Validators.pattern(FULL_EMAIL_REGEX),
      ],
      updateOn: 'blur',
    }),
    password: this.fb.nonNullable.control<string>('', {
      validators: [
        Validators.required,
        Validators.minLength(6),
        Validators.maxLength(128),
        notOnlySpaces(),
      ],
      updateOn: 'change',
    }),
  });

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

  constructor() {}
  ngOnInit(): void {}

  togglePasswordVisibility(): void {
    this.passwordVisible = !this.passwordVisible;
  }

  // 👇 Ahora loguea el /me en consola
  me() {
    this.auth.GetMe().subscribe({
      next: (user) => {
        console.log('[GET /auth/me] → user:', user);
      },
      error: (err) => {
        this.showIonAlert('Oops...', err?.message ?? 'Error inesperado');
        console.error('[GET /auth/me] error:', err);
      },
    });
  }

  onEmailBlur() {
    const current = (this.emailCtrl.value ?? '').toLowerCase().trim();
    const patched = appendDefaultTldIfNeeded(current, '.com');
    if (patched !== current) {
      this.emailCtrl.setValue(patched, { emitEvent: false });
      this.emailCtrl.updateValueAndValidity();
    }
  }

  // estado del loading
  showLoading = false;

  login() {
    if (this.formLogin.invalid) {
      this.formLogin.markAllAsTouched();
      const fixed = appendDefaultTldIfNeeded(
        (this.emailCtrl.value ?? '').toLowerCase().trim(),
        '.com'
      );
      if (fixed !== this.emailCtrl.value) {
        this.emailCtrl.setValue(fixed, { emitEvent: false });
        this.emailCtrl.updateValueAndValidity();
      }
      if (this.formLogin.invalid) return;
    }

    let email = (this.emailCtrl.value ?? '').toLowerCase().trim();
    email = appendDefaultTldIfNeeded(email, '.com');
    const password = this.passwordCtrl.value ?? '';

    // mostrar spinner
    this.showLoading = true;

    this.auth.Login({ email, password }).subscribe({
      next: (user) => {
        console.log('[POST /auth/login] OK → luego /me:', user);
        this.showLoading = false; // cierra el loading
        setTimeout(() => {
          this.router.navigate(['/home']);
        }, 100);
      },
      error: (err) => {
        this.showLoading = false;
        this.showIonAlert('Oops...', err?.error?.message ?? 'Credenciales inválidas');
        console.error('[POST /auth/login] error:', err);
      },
    });
  }

  /** Muestra un ion-alert */
  private showIonAlert(header: string, message: string) {
    this.alertHeader = header;
    this.alertMessage = message;
    this.showAlert = true;
  }
}
