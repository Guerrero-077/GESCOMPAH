import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../../core/security/services/auth/auth.service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.css'
})
export class ResetPasswordComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);
  private sweet = inject(SweetAlertService);

  readonly FULL_EMAIL_REGEX = /^(?=.{1,254}$)(?=.{1,64}@)[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i;

  form = this.fb.nonNullable.group({
    email: this.fb.nonNullable.control<string>('', [
      Validators.required,
      Validators.email,
      Validators.maxLength(254),
      notOnlySpaces(),
      Validators.pattern(this.FULL_EMAIL_REGEX)
    ])
  });

  get emailCtrl() { return this.form.get('email')!; }

  isInvalid(ctrl: AbstractControl | null) {
    return !!ctrl && ctrl.invalid && (ctrl.dirty || ctrl.touched);
  }

  async submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.sweet.showNotification('Advertencia', 'Por favor ingrese un correo válido.', 'warning');
      return;
    }

    const email = this.emailCtrl.value.trim();
    this.sweet.showLoading('Enviando código', 'Por favor, espera...');
    this.auth.RequestPasswordReset(email).subscribe({
      next: async () => {
        await this.sweet.hideLoading();
        await this.sweet.success('Enviamos un código a tu correo (si existe).');
        this.router.navigate(['/', 'auth', 'password_reset', 'confirm'], { queryParams: { email } });
      },
      error: async (err) => {
        await this.sweet.hideLoading();
        const message = err?.error?.detail || err?.error?.title || err?.message || 'Error al enviar el código.';
        await this.sweet.error(message);
      }
    });
  }
}

function notOnlySpaces() {
  return (c: AbstractControl): ValidationErrors | null => {
    const v = (c.value ?? '').toString();
    return v.trim().length === 0 ? { onlySpaces: true } : null;
  };
}
