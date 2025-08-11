import { Component, inject, OnInit } from '@angular/core'; // Add OnInit
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import Swal from 'sweetalert2';
import { AuthService } from '../../../../core/service/auth/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  imports: [
    ReactiveFormsModule,
    CommonModule,
    RouterModule
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit { // Implement OnInit
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  constructor() {
  }

  ngOnInit(): void {
  }

  formLogin = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
  });

  me() {
    this.auth.GetMe().subscribe({
      next: (data) => {},
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
          this.router.navigate(['/Admin/dashboard']);

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
