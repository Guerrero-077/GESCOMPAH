import { CommonModule } from '@angular/common';
import { Component, computed, DestroyRef, inject, Inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatStepperModule } from '@angular/material/stepper';
import { BehaviorSubject, distinctUntilChanged, tap, switchMap, of, finalize, map, catchError } from 'rxjs';
import { RoleSelectModel } from '../../../security/models/role.models';
import { RoleService } from '../../../security/services/role/role.service';
import { CitySelectModel } from '../../../setting/models/city.models';
import { CityService } from '../../../setting/services/city/city.service';
import { DepartmentStore } from '../../../setting/services/department/department.store';
import { TenantFormData } from '../../models/tenants.models';


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
export class TenantsFormDialogComponent implements OnInit {

  private readonly fb = inject(FormBuilder);
  private readonly destroyRef = inject(DestroyRef);
  private readonly deptStore = inject(DepartmentStore);
  private readonly cityService = inject(CityService);
  private readonly roleService = inject(RoleService);

  constructor(
    private readonly dialogRef: MatDialogRef<TenantsFormDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public readonly data: TenantFormData
  ) { }

  submitting = signal(false);
  submitError = signal<string | null>(null);
  loadingCities = signal(false);
  loadingRoles = signal(false);
  isCreate = computed(() => this.data?.mode !== 'edit');

  departments$ = this.deptStore.departments$;
  private _cities = new BehaviorSubject<CitySelectModel[]>([]);
  cities$ = this._cities.asObservable();
  roles$ = new BehaviorSubject<RoleSelectModel[]>([]);

  compareById = (a: any, b: any) => Number(a) === Number(b);

  form: FormGroup = this.fb.group({
    // Ubicación
    location: this.fb.group({
      departmentId: [null, Validators.required], // se deshabilita/limpia en edición
      cityId: [{ value: null, disabled: true }, [Validators.required, Validators.min(1)]],
    }),

    // Persona
    person: this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.maxLength(50)]],
      document: ['', Validators.required],
      phone: ['', Validators.required],
      address: ['', Validators.required],
    }),

    // Cuenta
    account: this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: [''],
      confirmPassword: ['']
    }, { validators: passwordsMatch('password', 'confirmPassword') }),

    // Rol en el form raíz
    roleId: [null, [Validators.required, Validators.min(1)]],

    // Estado
    active: [true]
  });

  get locationGroup(): FormGroup { return this.form.get('location') as FormGroup; }
  get personGroup(): FormGroup { return this.form.get('person') as FormGroup; }
  get accountGroup(): FormGroup { return this.form.get('account') as FormGroup; }

  ngOnInit(): void {
    const deptCtrl = this.locationGroup.get('departmentId')!;
    const cityCtrl = this.locationGroup.get('cityId')!;

    if (this.isCreate()) {
      // Password requerido solo en creación
      this.accountGroup.get('password')?.addValidators([
        Validators.required, Validators.minLength(8), hasUpper(), hasLower(), hasDigit(), hasSymbol()
      ]);

      // Cadena Departamento → Ciudades (CREACIÓN)
      deptCtrl.valueChanges.pipe(
        takeUntilDestroyed(this.destroyRef),
        distinctUntilChanged(),
        tap((deptId) => {
          cityCtrl.reset({ value: null, disabled: !deptId }, { emitEvent: false });
          if (deptId) cityCtrl.enable({ emitEvent: false }); else cityCtrl.disable({ emitEvent: false });
          this._cities.next([]);
          this.loadingCities.set(!!deptId);
        }),
        switchMap((deptId: number | string | null) => {
          const id = deptId == null ? null : Number(deptId);
          if (!id || Number.isNaN(id)) {
            return of([]).pipe(finalize(() => this.loadingCities.set(false)));
          }
          return this.cityService.getCitiesByDepartment(id).pipe(
            map(toCityList),
            catchError(() => of([])),
            finalize(() => this.loadingCities.set(false))
          );
        })
      ).subscribe((list: CitySelectModel[]) => this._cities.next(list));
    } else {
      // ======= EDICIÓN =======
      // Departamento NO participa en validación ni UI
      deptCtrl.clearValidators();
      deptCtrl.setErrors(null);
      deptCtrl.setValue(null, { emitEvent: false });
      deptCtrl.disable({ emitEvent: false });

      // La ciudad siempre habilitada en edición
      cityCtrl.enable({ emitEvent: false });

      const presetCity = Number(this.data?.tenant?.cityId ?? 0) || null;

      // Cargar TODAS las ciudades (como en PersonComponent)
      this.loadingCities.set(true);
      this.cityService.getAll().pipe(
        map(toCityList),
        catchError(() => of([])),
        finalize(() => this.loadingCities.set(false))
      ).subscribe(list => {
        this._cities.next(list);
        if (presetCity) {
          cityCtrl.setValue(presetCity, { emitEvent: false });
        }
      });
    }

    // Cargar roles y preseleccionar
    this.loadRoles();

    // Patch inicial
    this.patchInitialData();

    // Si en creación ya venía departamento, dispara la cadena
    const presetDept = this.locationGroup.get('departmentId')?.value;
    if (this.isCreate() && presetDept) deptCtrl.updateValueAndValidity({ emitEvent: true });
  }

  private loadRoles(): void {
    this.loadingRoles.set(true);
    this.roleService.getAll()
      .pipe(
        finalize(() => this.loadingRoles.set(false)),
        catchError(() => {
          this.roles$.next([]);
          return of([]);
        })
      )
      .subscribe((roles: RoleSelectModel[]) => {
        this.roles$.next(roles);

        const roleName = this.data?.tenant?.roleName ?? null;
        if (roleName) {
          const match = roles.find(r => r.name?.toLowerCase().trim() === roleName.toLowerCase().trim());
          if (match) this.form.get('roleId')?.setValue(match.id, { emitEvent: false });
        }

        if (this.data?.tenant?.roleIds?.length) {
          this.form.get('roleId')?.setValue(this.data.tenant.roleIds[0], { emitEvent: false });
        }
      });
  }

  private patchInitialData(): void {
    const t = this.data?.tenant ?? {};

    // Ubicación
    this.locationGroup.patchValue({
      departmentId: this.isCreate() ? (t.departmentId ?? null) : null,
      cityId: t.cityId ?? null
    }, { emitEvent: false });

    // Persona
    this.personGroup.patchValue({
      firstName: t.firstName ?? '',
      lastName: t.lastName ?? '',
      document: t.document ?? '',
      phone: String(t.phone ?? '').trim(),
      address: t.address ?? ''
    }, { emitEvent: false });

    // Cuenta
    this.accountGroup.patchValue({
      email: String(t.email ?? '').trim().toLowerCase(),
      password: '',
      confirmPassword: ''
    }, { emitEvent: false });

    // Estado
    this.form.get('active')?.setValue(t.active ?? true, { emitEvent: false });
  }

  submit(): void {
    this.submitError.set(null);

    // Normaliza email
    const emailCtrl = this.accountGroup.get('email')!;
    emailCtrl.setValue(String(emailCtrl.value ?? '').trim().toLowerCase(), { emitEvent: false });

    // En edición, si password vacío, quitar error de confirmación
    if (!this.isCreate() && !this.accountGroup.get('password')?.value) {
      this.accountGroup.setErrors(null);
    }

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);

    const loc = this.locationGroup.value;
    const p = this.personGroup.value;
    const acc = this.accountGroup.value;
    const roleId = this.form.get('roleId')?.value as number | null;
    const active = !!this.form.get('active')?.value;

    const base = {
      firstName: p.firstName,
      lastName: p.lastName,
      document: p.document,
      phone: p.phone,
      address: p.address,
      departmentId: Number(loc.departmentId ?? 0) || null, // solo para creación
      cityId: Number(loc.cityId ?? 0) || null,
      email: acc.email,
      roleIds: roleId ? [roleId] : [],
      active
    };

    const payload = this.isCreate()
      ? { ...base, password: acc.password }
      : ({
        // CONTRATO UserUpdateDto
        personId: this.data?.tenant?.personId,
        firstName: base.firstName,
        lastName: base.lastName,
        document: base.document,
        phone: base.phone,
        address: base.address,
        cityId: base.cityId,
        email: base.email,
        password: acc.password || null,
        roleIds: base.roleIds
      });

    this.dialogRef.close(payload);
    this.submitting.set(false);
  }

  cancel(): void {
    this.dialogRef.close(null);
  }
}

/* ====== utils ====== */
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

/* ========= VALIDADORES ========= */
function passwordsMatch(passwordKey: string, confirmKey: string) {
  return (group: AbstractControl): ValidationErrors | null => {
    const pass = group.get(passwordKey)?.value ?? '';
    const confirm = group.get(confirmKey)?.value ?? '';
    return pass && confirm && pass !== confirm ? { passwordsMismatch: true } : null;
  };
}
function hasUpper() { return (c: AbstractControl) => /[A-Z]/.test(String(c.value ?? '')) ? null : { upper: true }; }
function hasLower() { return (c: AbstractControl) => /[a-z]/.test(String(c.value ?? '')) ? null : { lower: true }; }
function hasDigit() { return (c: AbstractControl) => /\d/.test(String(c.value ?? '')) ? null : { digit: true }; }
function hasSymbol() { return (c: AbstractControl) => /[^A-Za-z0-9]/.test(String(c.value ?? '')) ? null : { symbol: true }; }
