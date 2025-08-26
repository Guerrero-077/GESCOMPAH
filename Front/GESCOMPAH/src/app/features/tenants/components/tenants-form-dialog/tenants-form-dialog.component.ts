// ...imports iguales...
import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, DestroyRef, Inject, signal, computed } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';
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
import { TenantsCreateModel, TenantsSelectModel, TenantFormData } from '../../models/tenants.models';

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

  // ====== Form ======
  form: FormGroup = this.fb.group({
    location: this.fb.group({
      departmentId: [null, Validators.required],
      cityId: [{ value: null, disabled: true }, [Validators.required, Validators.min(1)]],
    }),
    person: this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.maxLength(50)]],
      document: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(20)]],
      phone: ['', [Validators.required, Validators.pattern(/^\d{7,15}$/)]],
      address: ['', [Validators.required, Validators.maxLength(100)]],
    }),
    account: this.fb.group({
      email: ['', [Validators.required, Validators.email]],
    }),
    roleId: [null, [Validators.required, Validators.min(1)]],
    active: [true]
  });

  get locationGroup(): FormGroup { return this.form.get('location') as FormGroup; }
  get personGroup(): FormGroup { return this.form.get('person') as FormGroup; }
  get accountGroup(): FormGroup { return this.form.get('account') as FormGroup; }

  ngOnInit(): void {
    const deptCtrl = this.locationGroup.get('departmentId')!;
    const cityCtrl = this.locationGroup.get('cityId')!;

    if (this.isCreate()) {
      deptCtrl.valueChanges.pipe(
        takeUntilDestroyed(this.destroyRef),
        distinctUntilChanged(),
        tap((deptId) => {
          cityCtrl.reset({ value: null, disabled: !deptId }, { emitEvent: false });
          deptId ? cityCtrl.enable({ emitEvent: false }) : cityCtrl.disable({ emitEvent: false });
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
      deptCtrl.clearValidators();
      deptCtrl.setErrors(null);
      deptCtrl.setValue(null, { emitEvent: false });
      deptCtrl.disable({ emitEvent: false });

      cityCtrl.enable({ emitEvent: false });

      const presetCity = Number(this.data?.tenant?.cityId ?? 0) || null;
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

    this.loadRoles();
    this.patchInitialData();

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

        if (this.data?.tenant?.roles?.length) {
          const match = roles.find(r => this.data.tenant!.roles.includes(r.name ?? ''));
          if (match) this.form.get('roleId')?.setValue(match.id, { emitEvent: false });
        }
      });
  }

  private patchInitialData(): void {
    const t = this.data?.tenant as TenantsSelectModel;

    // Ubicación
    this.locationGroup.patchValue({
      departmentId: this.isCreate() ? null : null,
      cityId: t?.cityId ?? null
    }, { emitEvent: false });

    // Persona
    this.personGroup.patchValue({
      firstName: t?.personName?.split(' ')[0] ?? '',
      lastName: t?.personName?.split(' ')[1] ?? '',
      document: t?.personDocument ?? '',
      phone: t?.personPhone ?? '',
      address: t?.personAddress ?? ''
    }, { emitEvent: false });

    // Cuenta
    this.accountGroup.patchValue({
      email: t?.email?.toLowerCase() ?? ''
    }, { emitEvent: false });

    // Estado
    this.form.get('active')?.setValue(t?.active ?? true, { emitEvent: false });
  }


  submit(): void {
    this.submitError.set(null);

    const emailCtrl = this.accountGroup.get('email')!;
    emailCtrl.setValue(String(emailCtrl.value ?? '').trim().toLowerCase(), { emitEvent: false });

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      Object.values(this.form.controls).forEach(c => c.updateValueAndValidity());
      return;
    }

    this.submitting.set(true);

    const loc = this.locationGroup.value;
    const p = this.personGroup.value;
    const acc = this.accountGroup.value;
    const roleId = this.form.get('roleId')?.value as number | null;
    const active = !!this.form.get('active')?.value;

    const payload: TenantsCreateModel | Partial<TenantsSelectModel> = this.isCreate()
      ? {
        firstName: p.firstName,
        lastName: p.lastName,
        document: String(p.document), // Forzar a string
        phone: String(p.phone),
        address: p.address,
        cityId: Number(loc.cityId),
        email: acc.email,
        roleIds: roleId ? [roleId] : []
      }
      : {
        id: this.data?.tenant?.id!,
        personId: this.data?.tenant?.personId!,
        personName: `${p.firstName} ${p.lastName}`,
        personDocument: p.document,
        personPhone: p.phone,
        personAddress: p.address,
        email: acc.email,
        cityId: Number(loc.cityId),
        active,
        roles: []
      };

    this.dialogRef.close(payload);
    this.submitting.set(false);
  }

  cancel(): void {
    this.dialogRef.close(null);
  }

  markGroupAsTouched(group: FormGroup) {
    group.markAllAsTouched();
    Object.values(group.controls).forEach(c => c.updateValueAndValidity());
  }

  private errorMessages: Record<string, string> = {
    required: 'Requerido',
    email: 'Correo inválido',
    minlength: 'Mín. 8 caracteres',
    maxlength: 'Máx. 50 caracteres',
    pattern: 'Formato inválido',
    min: 'Valor inválido',
  };

  getFirstError(control: AbstractControl | null, order: string[]): string | null {
    if (!control || !control.errors) return null;
    for (const key of order) {
      if (control.hasError(key)) return this.errorMessages[key] ?? key;
    }
    const firstKey = Object.keys(control.errors)[0];
    return firstKey ? (this.errorMessages[firstKey] ?? firstKey) : null;
  }
}

// ========== UTILS ==========
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
