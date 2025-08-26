// ...imports iguales...
import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, DestroyRef, Inject, signal, computed } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
  ReactiveFormsModule, FormBuilder, FormGroup, Validators, AbstractControl, ValidationErrors, ValidatorFn, FormControl
} from '@angular/forms';
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

  compareById = (a: any, b: any) => String(a ?? '') === String(b ?? '');

  /* =======================
     VALIDADORES REUTILIZABLES
     ======================= */

  private onlySpaces(): ValidatorFn {
    return (c: AbstractControl): ValidationErrors | null =>
      (c.value ?? '').toString().trim().length === 0 ? { onlySpaces: true } : null;
  }

  private alphaHumanName(): ValidatorFn {
    const rx = /^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ]+(?:[ '’-][A-Za-zÁÉÍÓÚÜÑáéíóúüñ]+)*$/;
    return (c: AbstractControl) => {
      const v = (c.value ?? '').toString().trim();
      return v && rx.test(v) ? null : { alphaHuman: true };
    };
  }

  private numericString(minLen: number, maxLen: number): ValidatorFn {
    const rx = new RegExp(`^\\d{${minLen},${maxLen}}$`);
    return (c: AbstractControl) => {
      const v = String(c.value ?? '').trim();
      return v && rx.test(v) ? null : { numericString: true };
    };
  }

  private phoneE164Like(): ValidatorFn {
    const rx = /^\+?\d{7,15}$/;
    return (c: AbstractControl) => {
      const v = String(c.value ?? '').trim();
      return v && rx.test(v) ? null : { phone: true };
    };
  }

  /** Email debe tener dominio con punto (TLD). Ej: user@dominio.com */
  private emailWithDotTld(): ValidatorFn {
    return (c: AbstractControl): ValidationErrors | null => {
      const v = String(c.value ?? '').trim();
      if (!v) return null; // Otros validators (required) se encargan
      const at = v.indexOf('@');
      if (at < 0) return null; // si no hay @, que falle Validators.email primero
      const domain = v.slice(at + 1);
      if (!domain) return { domainMissing: true };
      // Solo chars válidos de dominio
      if (!/^[A-Za-z0-9.-]+$/.test(domain)) return { domainInvalid: true };
      if (!domain.includes('.')) return { tldMissing: true };
      const lastLabel = domain.split('.').pop() || '';
      // TLD común de 2 a 24 caracteres
      if (lastLabel.length < 2 || lastLabel.length > 24) return { tldInvalid: true };
      return null;
    };
  }

  /* =======================
     FORM ROOT
     ======================= */
  form: FormGroup = this.fb.group({
    location: this.fb.group({
      departmentId: [null, Validators.required],
      cityId: [{ value: null, disabled: true }, [Validators.required, Validators.min(1)]],
    }),

    person: this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(50), this.onlySpaces(), this.alphaHumanName()]],
      lastName: ['', [Validators.required, Validators.maxLength(50), this.onlySpaces(), this.alphaHumanName()]],
      document: ['', [Validators.required, this.numericString(5, 20)]],
      phone: ['', [Validators.required, this.phoneE164Like()]],
      address: ['', [Validators.required, Validators.maxLength(100), this.onlySpaces()]],
    }),

    account: this.fb.group({
      email: ['', [Validators.required, Validators.email, this.emailWithDotTld()]],
    }),

    roleId: [null, [Validators.required, Validators.min(1)]],
    active: [true]
  });

  // GETTERS rápidos
  get locationGroup(): FormGroup { return this.form.get('location') as FormGroup; }
  get personGroup(): FormGroup { return this.form.get('person') as FormGroup; }
  get accountGroup(): FormGroup { return this.form.get('account') as FormGroup; }

  // ✅ Getter tipado para usar en [formControl]
  get roleIdCtrl(): FormControl {
    return this.form.get('roleId') as FormControl;
  }

  ngOnInit(): void {
    const deptCtrl = this.locationGroup.get('departmentId')!;
    const cityCtrl = this.locationGroup.get('cityId')!;

    if (this.isCreate()) {
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
      ).subscribe((list: CitySelectModel[]) => {
        this._cities.next(list);

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
        if (presetCity) cityCtrl.setValue(presetCity, { emitEvent: false });
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
          if (match) this.roleIdCtrl.setValue(match.id, { emitEvent: false });
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
      departmentId: this.isCreate() ? null : null,
      cityId: t?.cityId ?? null
    }, { emitEvent: false });

    this.form.get('active')?.setValue(t?.active ?? true, { emitEvent: false });
  }

  /** Normaliza y auto-completa TLD si falta (agrega .com). */
  fixEmail(): void {
    const emailCtrl = this.accountGroup.get('email')!;
    let v = String(emailCtrl.value ?? '').trim().toLowerCase();
    if (!v) return;
    const at = v.indexOf('@');
    if (at < 0) {
      // no tocamos nada: dejar que falle Validators.email / required
      emailCtrl.setValue(v, { emitEvent: false });
      return;
    }
    const local = v.slice(0, at);
    const domain = v.slice(at + 1);
    if (!domain) {
      // dominio vacío -> inválido (validator lo marcará)
      emailCtrl.setValue(`${local}@`, { emitEvent: false });
      return;
    }
    // si no hay punto en el dominio, agregar .com
    if (!domain.includes('.')) {
      v = `${local}@${domain}.com`;
      emailCtrl.setValue(v, { emitEvent: false });
    } else {
      emailCtrl.setValue(`${local}@${domain}`, { emitEvent: false });
    }
  }

  submit(): void {
    this.submitError.set(null);

    // Normalizaciones previas
    this.fixEmail(); // <-- asegura .com si falta
    const emailCtrl = this.accountGroup.get('email')!;
    emailCtrl.setValue(String(emailCtrl.value ?? '').trim().toLowerCase(), { emitEvent: false });

    const fn = this.personGroup.get('firstName')!;
    const ln = this.personGroup.get('lastName')!;
    fn.setValue(String(fn.value ?? '').trim(), { emitEvent: false });
    ln.setValue(String(ln.value ?? '').trim(), { emitEvent: false });

    const doc = this.personGroup.get('document')!;
    const phone = this.personGroup.get('phone')!;
    doc.setValue(String(doc.value ?? '').trim(), { emitEvent: false });
    phone.setValue(String(phone.value ?? '').trim(), { emitEvent: false });

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      Object.values((this.form.controls)).forEach(c => c.updateValueAndValidity());
      return;
    }

    this.submitting.set(true);

    const loc = this.locationGroup.value;
    const p = this.personGroup.value;
    const acc = this.accountGroup.value;
    const roleId = this.roleIdCtrl.value as number | null;
    const active = !!this.form.get('active')?.value;

    const payload: TenantsCreateModel | Partial<TenantsSelectModel> = this.isCreate()
      ? {
        firstName: p.firstName,
        lastName: p.lastName,
        document: String(p.document),
        phone: String(p.phone),
        address: p.address,
        cityId: Number(loc.cityId),
        email: acc.email,
        roleIds: roleId ? [roleId] : []
      }
      : {
        id: this.data?.tenant?.id!,
        personId: this.data?.tenant?.personId!,
        personName: `${p.firstName} ${p.lastName}`.trim(),
        personDocument: String(p.document),
        personPhone: String(p.phone),
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

  /* ========== MENSAJES ========== */
  private errorMessages: Record<string, string> = {
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
    numericString: 'Debe contener solo dígitos (longitud permitida)',
    phone: 'Teléfono inválido (7–15 dígitos, opcional +)',
  };

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

/* ========== UTILS ========== */
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
