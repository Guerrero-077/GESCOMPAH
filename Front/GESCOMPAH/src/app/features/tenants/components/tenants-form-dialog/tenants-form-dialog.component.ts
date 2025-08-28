import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import {
  ReactiveFormsModule, FormBuilder, FormGroup, Validators,
  AbstractControl, ValidationErrors, ValidatorFn, FormControl
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatStepperModule } from '@angular/material/stepper';
import { BehaviorSubject, catchError, distinctUntilChanged, finalize, map, of, switchMap, tap } from 'rxjs';

import { RoleSelectModel } from '../../../security/models/role.models';
import { RoleService } from '../../../security/services/role/role.service';
import { CitySelectModel } from '../../../setting/models/city.models';
import { CityService } from '../../../setting/services/city/city.service';
import { DepartmentStore } from '../../../setting/services/department/department.store';

import { TenantFormData, TenantsCreateModel, TenantsSelectModel, TenantsUpdateModel } from '../../models/tenants.models';

/* =======================
   Validadores reutilizables (puros)
   ======================= */
function notOnlySpaces(): ValidatorFn {
  return (c: AbstractControl): ValidationErrors | null => {
    const v = (c.value ?? '') as string;
    if (v == null) return null;
    return v.trim().length === 0 ? { onlySpaces: true } : null;
  };
}
function alphaHumanName(): ValidatorFn {
  const rx = /^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ]+(?:[ '’-][A-Za-zÁÉÍÓÚÜÑáéíóúüñ]+)*$/;
  return (c: AbstractControl): ValidationErrors | null => {
    const v = String(c.value ?? '').trim();
    if (!v) return null;
    return rx.test(v) ? null : { alphaHuman: true };
  };
}
function numericString(minLen: number, maxLen: number): ValidatorFn {
  const rx = new RegExp(`^\d{${minLen},${maxLen}}$`);
  return (c: AbstractControl): ValidationErrors | null => {
    const v = String(c.value ?? '').trim();
    if (!v) return null;
    return rx.test(v) ? null : { numericString: true };
  };
}
function phoneE164Like(): ValidatorFn {
  const rx = /^\+?\d{7,15}$/;
  return (c: AbstractControl): ValidationErrors | null => {
    const v = String(c.value ?? '').trim();
    if (!v) return null;
    return rx.test(v) ? null : { phone: true };
  };
}
function emailWithDotTld(): ValidatorFn {
  return (c: AbstractControl): ValidationErrors | null => {
    const v = String(c.value ?? '').trim();
    if (!v) return null;
    const at = v.indexOf('@');
    if (at < 0) return null; // deja que falle Validators.email primero
    const domain = v.slice(at + 1);
    if (!domain) return { domainMissing: true };
    if (!/^[A-Za-z0-9.-]+$/.test(domain)) return { domainInvalid: true };
    if (!domain.includes('.')) return { tldMissing: true };
    const lastLabel = domain.split('.').pop() || '';
    if (lastLabel.length < 2 || lastLabel.length > 24) return { tldInvalid: true };
    return null;
  };
}

/* =======================
   Utils
   ======================= */
const toCityList = (res: any): CitySelectModel[] => {
  const raw = Array.isArray(res) ? res
    : Array.isArray(res?.data) ? res.data
      : Array.isArray(res?.items) ? res.items
        : Array.isArray(res?.result) ? res.result
          : [];
  return raw.map((c: any) => ({
    id: Number(c.id ?? c.cityId ?? c.value),
    name: String(c.name ?? c.cityName ?? c.label ?? '').trim(),
  })).filter((c: any) => !!c.id && !!c.name);
};

@Component({
  selector: 'app-tenants-form-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatStepperModule
  ],
  templateUrl: './tenants-form-dialog.component.html',
  styleUrls: ['./tenants-form-dialog.component.css'],
})
export class TenantsFormDialogComponent {
  // Inyección con inject()
  private readonly fb = inject(FormBuilder);
  private readonly deptStore = inject(DepartmentStore);
  private readonly cityService = inject(CityService);
  private readonly roleService = inject(RoleService);
  private readonly dialogRef = inject(MatDialogRef<TenantsFormDialogComponent>);
  readonly data = inject<TenantFormData>(MAT_DIALOG_DATA, { optional: true }) ?? { mode: 'create' };

  // Estado
  isEdit = this.data?.mode === 'edit';
  isLoading = false;
  loadingCities = false;
  loadingRoles = false;

  // Catálogos
  readonly departments$ = this.deptStore.departments$;
  private _cities$ = new BehaviorSubject<CitySelectModel[]>([]);
  readonly cities$ = this._cities$.asObservable();
  private _roles$ = new BehaviorSubject<RoleSelectModel[]>([]);
  readonly roles$ = this._roles$.asObservable();

  // Form
  form: FormGroup = this.fb.group({
    location: this.fb.group({
      departmentId: [null, Validators.required],
      cityId: [{ value: null, disabled: true }, [Validators.required, Validators.min(1)]],
    }),
    person: this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(50), notOnlySpaces(), alphaHumanName()]],
      lastName: ['', [Validators.required, Validators.maxLength(50), notOnlySpaces(), alphaHumanName()]],
      document: ['', [Validators.required, numericString(5, 20)]], // solo se usa en CREATE
      phone: ['', [Validators.required, phoneE164Like()]],
      address: ['', [Validators.required, Validators.maxLength(100), notOnlySpaces()]],
    }),
    account: this.fb.group({
      email: ['', [Validators.required, Validators.email, emailWithDotTld()]],
    }),
    roleId: [null, [Validators.required, Validators.min(1)]],
    active: [true], // solo UI (no viaja en update del back actual)
  });

  // Getters
  get locationGroup(): FormGroup { return this.form.get('location') as FormGroup; }
  get personGroup(): FormGroup { return this.form.get('person') as FormGroup; }
  get accountGroup(): FormGroup { return this.form.get('account') as FormGroup; }
  get roleIdCtrl(): FormControl { return this.form.get('roleId') as FormControl; }

  // Compare helper para mat-select
  compareById = (a: any, b: any) => String(a ?? '') === String(b ?? '');

  // Errores
  private readonly errorMessages: Record<string, string> = {
    required: 'Requerido',
    email: 'Correo inválido',
    domainMissing: 'Falta el dominio después de @',
    domainInvalid: 'Dominio inválido',
    tldMissing: 'Falta el TLD (por ej. .com)',
    tldInvalid: 'TLD inválido',
    minlength: 'Longitud mínima no válida',
    maxlength: 'Longitud máxima excedida',
    pattern: 'Formato inválido',
    min: 'Selecciona un valor válido',
    onlySpaces: 'No puede ser solo espacios',
    alphaHuman: 'Solo letras y separadores válidos',
    numericString: 'Solo dígitos (longitud permitida)',
    phone: 'Teléfono inválido (7–15 dígitos, opcional +)',
  };

  // Init
  ngOnInit(): void {
    if (this.isEdit) {
      console.log('Datos iniciales para editar:', this.data.tenant);
    }
    this.configureEditMode();
    this.setupCityCascading();
    this.loadRoles();
    this.patchInitialData();
  }

  private configureEditMode(): void {
    if (!this.isEdit) return;
    const deptCtrl = this.locationGroup.get('departmentId')!;
    const cityCtrl = this.locationGroup.get('cityId')!;
    deptCtrl.clearValidators();
    deptCtrl.setErrors(null);
    deptCtrl.setValue(null, { emitEvent: false });
    deptCtrl.disable({ emitEvent: false });
    cityCtrl.enable({ emitEvent: false });

    // Deshabilitar el campo 'document' en modo edición, ya que no se puede cambiar
    this.personGroup.get('document')?.disable({ emitEvent: false });
  }

  private setupCityCascading(): void {
    const deptCtrl = this.locationGroup.get('departmentId')!;
    const cityCtrl = this.locationGroup.get('cityId')!;

    if (!this.isEdit) {
      deptCtrl.valueChanges.pipe(
        distinctUntilChanged(),
        tap((deptId) => {
          cityCtrl.reset({ value: null, disabled: !deptId }, { emitEvent: false });
          if (deptId) cityCtrl.enable({ emitEvent: false }); else cityCtrl.disable({ emitEvent: false });
          this._cities$.next([]);
          this.loadingCities = !!deptId;
        }),
        switchMap((deptId: number | string | null) => {
          const id = deptId == null ? null : Number(deptId);
          if (!id || Number.isNaN(id)) {
            return of([]).pipe(finalize(() => this.loadingCities = false));
          }
          return this.cityService.getCitiesByDepartment(id).pipe(
            map(toCityList),
            catchError(() => of([])),
            finalize(() => this.loadingCities = false)
          );
        })
      ).subscribe((list: CitySelectModel[]) => {
        this._cities$.next(list);

        if (this.locationGroup.get('departmentId')?.value) {
          cityCtrl.enable({ emitEvent: false });
          if (list.length === 0) {
            cityCtrl.setErrors({ required: true });
            cityCtrl.markAsTouched();
          } else {
            const v = Number(cityCtrl.value ?? 0);
            if (!v || v < 1) cityCtrl.setErrors({ required: true });
            else if (cityCtrl.valid) cityCtrl.setErrors(null);
          }
        } else {
          cityCtrl.disable({ emitEvent: false });
        }
      });
    } else {
      // Edición: carga plana de ciudades y preselección
      this.loadingCities = true;
      const presetCity = Number(this.data?.tenant?.cityId ?? 0) || null;
      this.cityService.getAll().pipe(
        map(toCityList),
        catchError(() => of([])),
        finalize(() => this.loadingCities = false)
      ).subscribe(list => {
        this._cities$.next(list);
        if (presetCity) cityCtrl.setValue(presetCity, { emitEvent: false });
      });
    }
  }

  private loadRoles(): void {
    this.loadingRoles = true;
    this.roleService.getAll()
      .pipe(
        finalize(() => this.loadingRoles = false),
        catchError(() => {
          this._roles$.next([]);
          return of([]);
        })
      )
      .subscribe((roles: RoleSelectModel[]) => {
        this._roles$.next(roles);

        // Preselección robusta en edición (por nombre, case-insensitive)
        if (this.isEdit && this.data?.tenant?.roles?.length) {
          const wanted = new Set(this.data.tenant.roles.map(r => String(r).trim().toLowerCase()));
          const match = roles.find(r => wanted.has(String(r.name ?? '').trim().toLowerCase()));
          if (match) {
            this.roleIdCtrl.setValue(match.id, { emitEvent: false });
            this.roleIdCtrl.updateValueAndValidity({ emitEvent: false });
          }
        }
      });
  }

  private patchInitialData(): void {
    const t = this.data?.tenant as TenantsSelectModel | undefined;

    const [firstName, ...rest] = String(t?.personName ?? '').trim().split(/\s+/);
    const lastName = rest.join(' ');

    this.personGroup.patchValue({
      firstName: firstName ?? '',
      lastName: lastName ?? '',
      document: t?.personDocument ?? '',
      phone: t?.personPhone ?? '',
      address: t?.personAddress ?? ''
    }, { emitEvent: false });

    this.accountGroup.patchValue({
      email: (t?.email ?? '').trim().toLowerCase()
    }, { emitEvent: false });

    this.locationGroup.patchValue({
      departmentId: this.isEdit ? null : null,
      cityId: t?.cityId ?? null
    }, { emitEvent: false });

    this.form.get('active')?.setValue(t?.active ?? true, { emitEvent: false });
  }

  // Normalizaciones
  private onTrim(control: AbstractControl | null) {
    if (!control) return;
    const v = (control.value ?? '') as string;
    const trimmed = v.trim().replace(/\s+/g, ' ');
    if (trimmed !== v) control.setValue(trimmed);
  }
  public fixEmail(): void {
    const emailCtrl = this.accountGroup.get('email')!;
    let v = String(emailCtrl.value ?? '').trim().toLowerCase();
    if (!v) return;
    const at = v.indexOf('@');
    if (at < 0) {
      emailCtrl.setValue(v, { emitEvent: false });
      return;
    }
    const local = v.slice(0, at);
    const domain = v.slice(at + 1);
    if (!domain) {
      emailCtrl.setValue(`${local}@`, { emitEvent: false });
      return;
    }
    if (!domain.includes('.')) {
      v = `${local}@${domain}.com`;
      emailCtrl.setValue(v, { emitEvent: false });
    } else {
      emailCtrl.setValue(`${local}@${domain}`, { emitEvent: false });
    }
  }

  // Submit (DTOs alineados con el back)
  submit(): void {
    // Normalizaciones
    this.fixEmail();
    const fn = this.personGroup.get('firstName')!;
    const ln = this.personGroup.get('lastName')!;
    const doc = this.personGroup.get('document')!;
    const phone = this.personGroup.get('phone')!;
    const addr = this.personGroup.get('address')!;
    const emailCtrl = this.accountGroup.get('email')!;
    [fn, ln, doc, phone, addr, emailCtrl].forEach(c => {
      c.setValue(String(c.value ?? '').trim(), { emitEvent: false });
      this.onTrim(c);
    });

    if (this.form.invalid) {
      this.markAllTouched();
      return;
    }

    this.isLoading = true;

    const loc = this.locationGroup.value;
    const p = this.personGroup.value;
    const acc = this.accountGroup.value;

    // Resolver roleIds
    const roleId = this.roleIdCtrl.value;
    const roleIds = roleId ? [Number(roleId)] : [];

    // CREATE → el back espera también 'document'
    if (!this.isEdit) {
      const payload: TenantsCreateModel = {
        firstName: p.firstName,
        lastName: p.lastName,
        document: String(p.document),
        phone: String(p.phone),
        address: p.address,
        cityId: Number(loc.cityId),
        email: acc.email,
        roleIds
      };
      this.dialogRef.close(payload);
      this.isLoading = false;
      return;
    }

    // UPDATE → el back NO espera personId ni document
    const t = this.data!.tenant!;
    const payload: TenantsUpdateModel = {
      id: t.id,
      firstName: p.firstName,
      lastName: p.lastName,
      phone: String(p.phone),
      address: p.address,
      cityId: Number(loc.cityId),
      email: acc.email,
      roleIds
    };

    this.dialogRef.close(payload);
    this.isLoading = false;
  }

  cancel(): void {
    this.dialogRef.close(null);
  }

  // Helpers UI
  markAllTouched(): void {
    this.form.markAllAsTouched();
    Object.values(this.form.controls).forEach(c => c.updateValueAndValidity());
    Object.values(this.locationGroup.controls).forEach(c => c.updateValueAndValidity());
    Object.values(this.personGroup.controls).forEach(c => c.updateValueAndValidity());
    Object.values(this.accountGroup.controls).forEach(c => c.updateValueAndValidity());
  }

  getFirstError(control: AbstractControl | null, order: string[] = []): string | null {
    if (!control || !control.errors) return null;
    const keys = order.length ? order : Object.keys(control.errors);
    for (const key of keys) {
      if (control.hasError(key)) return this.errorMessages[key] ?? key;
    }
    const firstKey = Object.keys(control.errors)[0];
    return firstKey ? (this.errorMessages[firstKey] ?? firstKey) : null;
  }
}
