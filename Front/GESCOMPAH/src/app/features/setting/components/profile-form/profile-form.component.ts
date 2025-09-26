
import { Observable, of, forkJoin } from 'rxjs';
import { take, switchMap, tap } from 'rxjs/operators';

import { ProfileService } from '../../services/profile.service';
import { CityService } from '../../services/city/city.service';
import { DepartmentService } from '../../services/department/department.service';
import { PersonService } from '../../../security/services/person/person.service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';

import { PersonSelectModel, PersonUpdateModel } from '../../../security/models/person.models';
import { CitySelectModel } from '../../models/city.models';
import { DepartmentSelectModel } from '../../models/department.models';
import { AppValidators } from '../../../../shared/utils/AppValidators';
import { UserStore } from '../../../../core/security/services/permission/User.Store';
import { StandardButtonComponent } from '../../../../shared/components/standard-button/standard-button.component';
import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { FormErrorComponent } from '../../../../shared/components/form-error/form-error.component';

@Component({
  selector: 'app-profile-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatCardModule,
    MatProgressSpinnerModule,
    StandardButtonComponent,
    FormErrorComponent
  ],
  templateUrl: './profile-form.component.html',
  styleUrls: ['./profile-form.component.css']
})
export class ProfileFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private profileService = inject(ProfileService);
  private cityService = inject(CityService);
  private departmentService = inject(DepartmentService);
  private personService = inject(PersonService);
  private sweetAlertService = inject(SweetAlertService);
  private userStore = inject(UserStore);

  form!: FormGroup;
  cities$!: Observable<CitySelectModel[]>;
  departments$!: Observable<DepartmentSelectModel[]>;
  profile: PersonSelectModel | null = null;
  isLoading = false;
  isSaving = false;

  // Mapas de mensajes para validadores personalizados usados en la vista
  firstNameMessages = { alphaHuman: () => 'Los nombres solo pueden contener letras y espacios' } as const;
  lastNameMessages = { alphaHuman: () => 'Los apellidos solo pueden contener letras y espacios' } as const;
  documentMessages = { colombianDocument: () => 'El documento debe tener entre 7 y 10 dígitos numéricos' } as const;
  phoneMessages = { colombianPhone: () => 'Ingrese un número celular válido (debe empezar en 3 y tener 10 dígitos)' } as const;
  emailMessages = {
    domainMissing: () => 'El correo debe incluir un dominio (ej: @gmail.com)',
    tldMissing: () => 'El dominio debe incluir una extensión válida (.com, .co, etc.)'
  } as const;
  cityIdMessages = { min: () => 'Debe seleccionar una ciudad válida de la lista' } as const;

  ngOnInit(): void {
    this.initForm();
    this.loadDepartments();
    this.setupCityCascading();
    this.loadProfile();
  }

  private initForm(): void {
    this.form = this.fb.group({
      id: [null],
      firstName: ['', [Validators.required, AppValidators.alphaHumanName()]],
      lastName: ['', [Validators.required, AppValidators.alphaHumanName()]],
      document: ['', [Validators.required, AppValidators.colombianDocument()]],
      phone: ['', [Validators.required, AppValidators.colombianPhone()]],
      address: ['', [Validators.required, Validators.maxLength(100)]],
      departmentId: [null, [Validators.required]],
      cityId: [{ value: null, disabled: true }, [Validators.required]],
      email: ['', [Validators.required, Validators.email, AppValidators.emailWithDotTld()]]
    });
  }

  private loadDepartments(): void {
    this.departments$ = this.departmentService.getAll();
  }

  private setupCityCascading(): void {
    const departmentControl = this.form.get('departmentId');
    const cityControl = this.form.get('cityId');

    departmentControl?.valueChanges.pipe(
      switchMap(departmentId => {
        cityControl?.reset({ value: null, disabled: true });
        if (departmentId) {
          cityControl?.enable();
          return this.cityService.getCitiesByDepartment(departmentId);
        }
        return of([]);
      })
    ).subscribe(cities => {
      this.cities$ = of(cities);
    });
  }

  private loadProfile(): void {
    this.isLoading = true;

    const profile$ = this.profileService.getProfile();
    const cities$ = this.cityService.getAll();

    forkJoin([profile$, cities$, this.departments$]).pipe(
      take(1)
    ).subscribe({
      next: ([profile, cities, departments]) => {
        this.profile = profile;
        this.form.patchValue(profile);

        const city = cities.find(c => c.name === profile.cityName);
        if (city) {
          const department = departments.find(d => d.name === city.departmentName);
          if (department) {
            this.form.get('departmentId')?.setValue(department.id);
            this.form.get('cityId')?.setValue(city.id);
          }
        }
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading profile', err);
        this.sweetAlertService.showApiError(err, 'No se pudo cargar el perfil. Asegúrate de que tu usuario tenga una persona asociada.');
        this.isLoading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    if (!this.profile) {
      this.sweetAlertService.showNotification('Error', 'No hay un perfil para actualizar.', 'error');
      return;
    }

    this.isSaving = true;
    const formData = this.form.getRawValue();
    const updateDto: PersonUpdateModel = {
      id: this.profile.id,
      firstName: formData.firstName,
      lastName: formData.lastName,
      phone: formData.phone,
      address: formData.address,
      cityId: formData.cityId
    };

    this.personService.update(this.profile.id, updateDto).subscribe({
      next: () => {
        this.sweetAlertService.showNotification('Éxito', 'Perfil actualizado correctamente.', 'success');
        this.isSaving = false;
      },
      error: (err) => {
        console.error('Error updating profile', err);
        this.sweetAlertService.showApiError(err, 'No se pudo actualizar el perfil.');
        this.isSaving = false;
      }
    });
  }
}
