import { Component } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { AuthService } from '../../../../core/security/services/auth/auth.service';
import { UserStore } from '../../../../core/security/services/permission/User.Store';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { CommonModule } from '@angular/common';
import { MatInputModule } from "@angular/material/input";
import { MatIconModule } from "@angular/material/icon";
import { MatButtonModule } from "@angular/material/button";
import { MatFormFieldModule } from "@angular/material/form-field";
import { StandardButtonComponent } from '../../../../shared/components/standard-button/standard-button.component';
import { FormErrorComponent } from '../../../../shared/components/form-error/form-error.component';

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatInputModule, MatIconModule, MatButtonModule, MatFormFieldModule, StandardButtonComponent, FormErrorComponent],
  templateUrl: './change-password.component.html',
  styleUrl: './change-password.component.css'
})
export class ChangePasswordComponent {
  form: FormGroup;
  passwordVisible: boolean = false;

  // Mensajes específicos para validadores custom de la nueva contraseña
  newPasswordMessages = {
    upper: () => 'Debe incluir al menos una letra mayúscula',
    lower: () => 'Debe incluir al menos una letra minúscula',
    digit: () => 'Debe incluir al menos un número',
    symbol: () => 'Debe incluir al menos un símbolo especial'
  } as const;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private userStore: UserStore,
    private sweetAlertService: SweetAlertService
  ) {
    this.form = this.fb.group({
      currentPassword: ['', [Validators.required]],
      newPassword: ['', [
        Validators.required,
        Validators.minLength(8),
        hasUpper(),
        hasLower(),
        hasDigit(),
        hasSymbol()
      ]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: matchPasswords('newPassword', 'confirmPassword') });
  }

  togglePasswordVisibility() {
    this.passwordVisible = !this.passwordVisible;
  }

  async onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.sweetAlertService.showNotification('Advertencia', 'Por favor, completa todos los campos correctamente.', 'warning');
      return;
    }

    const userId = this.userStore.snapshot?.id;

    if (!userId) {
      this.sweetAlertService.showNotification('Error', 'Usuario no identificado', 'error');
      return;
    }

    this.sweetAlertService.showLoading('Cambiando Contraseña', 'Por favor, espera...');

    const dto = {
      userId,
      currentPassword: this.form.value.currentPassword,
      newPassword: this.form.value.newPassword
    };

    this.authService.ChangePassword(dto).subscribe({
      next: () => {
        this.sweetAlertService.hideLoading();
        this.sweetAlertService.showNotification('Éxito', 'Contraseña actualizada correctamente', 'success');
        this.form.reset();
      },
      error: (err) => {
        this.sweetAlertService.hideLoading();

        const errorMessage =
          err?.error?.detail ??
          err?.error?.message ??
          err?.error?.title ??
          'Error inesperado';

        if (errorMessage === 'La contraseña actual es incorrecta.') {
          this.form.get('currentPassword')?.setErrors({ incorrect: true });
          this.form.get('currentPassword')?.markAsTouched();
          this.form.get('currentPassword')?.updateValueAndValidity();
        }

        this.sweetAlertService.showApiError(err, errorMessage);
      }
    });
  }
}

// Validadores reutilizables
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
function hasSymbol() { return (c: AbstractControl) => /[\W_]/.test(c.value || '') ? null : { symbol: true }; }
