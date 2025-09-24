import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../../core/security/services/auth/auth.service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';

@Component({
  selector: 'app-confirm-reset-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './confirm-reset-password.component.html',
  styleUrl: './confirm-reset-password.component.css'
})
export class ConfirmResetPasswordComponent {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private auth = inject(AuthService);
  private sweet = inject(SweetAlertService);

  form = this.fb.nonNullable.group({
    email: this.fb.nonNullable.control<string>('', [Validators.required, Validators.email, Validators.maxLength(254), notOnlySpaces()]),
    code: this.fb.nonNullable.control<string>('', [Validators.required, Validators.pattern(/^\d{6}$/)]),
    newPassword: this.fb.nonNullable.control<string>('', [
      Validators.required,
      Validators.minLength(8),
      hasUpper(), hasLower(), hasDigit(), hasSymbol()
    ]),
    confirmPassword: this.fb.nonNullable.control<string>('', [Validators.required])
  }, { validators: matchPasswords('newPassword', 'confirmPassword') });

  get emailCtrl() { return this.form.get('email')!; }
  get codeCtrl() { return this.form.get('code')!; }
  get newPasswordCtrl() { return this.form.get('newPassword')!; }
  get confirmPasswordCtrl() { return this.form.get('confirmPassword')!; }

  constructor() {
    const email = this.route.snapshot.queryParamMap.get('email');
    if (email) this.emailCtrl.setValue(email);
  }

  isInvalid(ctrl: AbstractControl | null) {
    return !!ctrl && ctrl.invalid && (ctrl.dirty || ctrl.touched);
  }

  async submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.sweet.showNotification('Advertencia', 'Verifica los campos del formulario.', 'warning');
      return;
    }

    const dto = {
      email: this.emailCtrl.value.trim(),
      code: this.codeCtrl.value.trim(),
      newPassword: this.newPasswordCtrl.value
    };

    this.sweet.showLoading('Confirmando', 'Por favor, espera...');
    this.auth.ConfirmPasswordReset(dto).subscribe({
      next: async () => {
        await this.sweet.hideLoading();
        await this.sweet.success('Contraseña actualizada correctamente.');
        this.router.navigate(['/', 'auth', 'login']);
      },
      error: async (err) => {
        await this.sweet.hideLoading();
        const message = err?.error?.detail || err?.error?.title || err?.message || 'No se pudo actualizar la contraseña.';
        await this.sweet.error(message);
      }
    });
  }
}

function matchPasswords(passKey: string, confirmKey: string) {
  return (group: AbstractControl): ValidationErrors | null => {
    const pass = group.get(passKey)?.value;
    const conf = group.get(confirmKey)?.value;
    return pass && conf && pass !== conf ? { notMatching: true } : null;
  };
}

function hasUpper() { return (c: AbstractControl) => /[A-Z]/.test(c.value || '') ? null : { upper: true }; }
function hasLower() { return (c: AbstractControl) => /[a-z]/.test(c.value || '') ? null : { lower: true }; }
function hasDigit() { return (c: AbstractControl) => /\d/.test(c.value || '') ? null : { digit: true }; }
function hasSymbol() { return (c: AbstractControl) => /[^\da-zA-Z]/.test(c.value || '') ? null : { symbol: true }; }
function notOnlySpaces() { return (c: AbstractControl) => (c.value || '').toString().trim().length ? null : { onlySpaces: true }; }

