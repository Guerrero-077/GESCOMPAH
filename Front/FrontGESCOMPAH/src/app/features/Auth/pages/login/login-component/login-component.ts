import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import Swal from 'sweetalert2';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../../../Core/Auth/auth-service';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-login-component',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    CommonModule,
    RouterModule
  ],
  templateUrl: './login-component.html',
  styleUrl: './login-component.css'
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  formLogin = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
  });

  me() {
    this.auth.GetMe().subscribe({
      next: (data) => console.log(data),
      error: (err) => {
        Swal.fire({
          icon: 'error',
          title: 'Oops...',
          text: err?.message ?? 'Error inesperado'
        });
      }
    });
  }

  login() {
    if (this.formLogin.invalid) return;

    const { email, password } = this.formLogin.getRawValue();

    this.auth.Login({ email, password }).subscribe({
      next: (data) => {
        if (data != null) {
          this.router.navigateByUrl('/Establishment/ListEstablishment');

          console.log(this.me());

          Swal.fire({
            icon: 'success',
            title: '¡Hecho!',
            text: 'Login Exitoso!'
          });
        } else {
          Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Error creating user!'
          });
        }
      },
      error: (err) => {
        Swal.fire({
          icon: 'error',
          title: 'Oops...',
          text: err?.message ?? 'Error inesperado'
        });
      }
    });
  }
}