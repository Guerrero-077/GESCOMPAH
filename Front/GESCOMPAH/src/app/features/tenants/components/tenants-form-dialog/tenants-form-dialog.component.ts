import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import {
  ReactiveFormsModule, FormBuilder, FormGroup, Validators, FormControl, AbstractControl
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatStepperModule } from '@angular/material/stepper';
import { StandardButtonComponent } from '../../../../shared/components/standard-button/standard-button.component';
import { DocumentFormatDirective } from '../../../../shared/directives/document-format/document-format.directive';
import { BehaviorSubject, catchError, distinctUntilChanged, finalize, map, of, switchMap, tap } from 'rxjs';

import { RoleSelectModel } from '../../../security/models/role.models';
import { RoleService } from '../../../security/services/role/role.service';
import { CitySelectModel } from '../../../setting/models/city.models';
import { CityService } from '../../../setting/services/city/city.service';
import { DepartmentStore } from '../../../setting/services/department/department.store';

import {
  TenantFormData,
  TenantsCreateModel,
  TenantsSelectModel,
  TenantsUpdateModel
} from '../../models/tenants.models';

import { AppValidators as AV } from '../../../../shared/utils/AppValidators';
import { ErrorMessageService } from '../../../../shared/Services/forms/error-message.service';
import { FormUtilsService, buildEmailValidators } from '../../../../shared/Services/forms/form-utils.service';
import { CatalogsMapperService } from '../../../../shared/Services/mappers/catalogs-mapper.service';

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
    MatStepperModule,
    StandardButtonComponent,
    DocumentFormatDirective
  ],
  templateUrl: './tenants-form-dialog.component.html',
  styleUrls: ['./tenants-form-dialog.component.css'],
})
export class TenantsFormDialogComponent implements OnInit {
  private readonly fb: FormBuilder = inject(FormBuilder);
  private readonly deptStore: DepartmentStore = inject(DepartmentStore);
  private readonly cityService: CityService = inject(CityService);
  private readonly roleService: RoleService = inject(RoleService);
  private readonly dialogRef: MatDialogRef<TenantsFormDialogComponent> =
    inject(MatDialogRef<TenantsFormDialogComponent>);
  readonly data: TenantFormData =
    inject<TenantFormData>(MAT_DIALOG_DATA, { optional: true }) ?? { mode: 'create' };

  private readonly utils: FormUtilsService = inject(FormUtilsService);
  private readonly errMsg: ErrorMessageService = inject(ErrorMessageService);
  private readonly mapper: CatalogsMapperService = inject(CatalogsMapperService);

  isEdit = this.data?.mode === 'edit';
  isLoading = false;
  loadingCities = false;
  loadingRoles = false;

  readonly departments$ = this.deptStore.departments$;
  private _cities$ = new BehaviorSubject<CitySelectModel[]>([]);
  readonly cities$ = this._cities$.asObservable();
  private _roles$ = new BehaviorSubject<RoleSelectModel[]>([]);
  readonly roles$ = this._roles$.asObservable();

  form: FormGroup = this.fb.group({
    location: this.fb.group({
      departmentId: [null, Validators.required],
      cityId: [{ value: null, disabled: true }, [Validators.required, Validators.min(1)]],
    }),
    person: this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(50), AV.notOnlySpaces(), AV.alphaHumanName()]],
      lastName:  ['', [Validators.required, Validators.maxLength(50), AV.notOnlySpaces(), AV.alphaHumanName()]],
      document:  ['', [Validators.required, AV.colombianDocument()]], // no editable en EDIT
      phone:     ['', [Validators.required, AV.colombianPhone()]],
      address:   ['', [Validators.required, Validators.maxLength(100), AV.notOnlySpaces()]],
    }),
    account: this.fb.group({
      email: ['', buildEmailValidators(true)], // ✔️ mismo set de validaciones
    }),
    roleId: [null, [Validators.required, Validators.min(1)]],
    active: [true],
  });

  get locationGroup(): FormGroup { return this.form.get('location') as FormGroup; }
  get personGroup(): FormGroup { return this.form.get('person') as FormGroup; }
  get accountGroup(): FormGroup { return this.form.get('account') as FormGroup; }
  get roleIdCtrl(): FormControl { return this.form.get('roleId') as FormControl; }

  compareById = (a: unknown, b: unknown) => String(a ?? '') === String(b ?? '');

  ngOnInit(): void {
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
            map(res => this.mapper.toCityList(res)),
            catchError(() => of([])),
            finalize(() => this.loadingCities = false)
          );
        })
      ).subscribe((list: CitySelectModel[]) => {
        this._cities$.next(list);
        if (deptCtrl.value) {
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
      this.loadingCities = true;
      const presetCity = Number(this.data?.tenant?.cityId ?? 0) || null;
      this.cityService.getAll().pipe(
        map(res => this.mapper.toCityList(res)),
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
          return of([] as RoleSelectModel[]);
        })
      )
      .subscribe((roles: RoleSelectModel[]) => {
        this._roles$.next(roles);
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
    const first = (t as any)?.personFirstName ?? (t as any)?.firstName ?? '';
    const last  = (t as any)?.personLastName  ?? (t as any)?.lastName  ?? '';

    if (first || last) {
      this.personGroup.patchValue({
        firstName: first,
        lastName: last,
        document: t?.personDocument ?? '',
        phone: (t?.personPhone ?? '').replace(/^\+57/, ''),
        address: t?.personAddress ?? ''
      }, { emitEvent: false });
    } else {
      const split = this.utils.splitNameEs(String(t?.personName ?? ''));
      this.personGroup.patchValue({
        firstName: split.firstName,
        lastName: split.lastName,
        document: t?.personDocument ?? '',
        phone: (t?.personPhone ?? '').replace(/^\+57/, ''),
        address: t?.personAddress ?? ''
      }, { emitEvent: false });
    }

    this.accountGroup.patchValue({
      email: (t?.email ?? '').trim().toLowerCase()
    }, { emitEvent: false });

    this.locationGroup.patchValue({
      departmentId: this.isEdit ? null : null,
      cityId: t?.cityId ?? null
    }, { emitEvent: false });

    this.form.get('active')?.setValue(t?.active ?? true, { emitEvent: false });
  }

  submit(): void {
    const fn = this.personGroup.get('firstName')!;
    const ln = this.personGroup.get('lastName')!;
    const doc = this.personGroup.get('document')!;
    const phone = this.personGroup.get('phone')!;
    const addr = this.personGroup.get('address')!;
    const emailCtrl = this.accountGroup.get('email')!;

    // ✔️ Normalizar SIEMPRE
    [fn, ln, doc, phone, addr].forEach(c => this.utils.normalizeWhitespace(c));
    this.utils.coerceEmailTld(emailCtrl); // ✔️ auto .com aquí también

    if (this.form.invalid) {
      this.markAllTouched();
      return;
    }

    this.isLoading = true;

    const loc = this.locationGroup.value as { cityId: number | null };
    const p   = this.personGroup.value as any;
    const acc = this.accountGroup.value as any;

    const roleId = this.roleIdCtrl.value;
    const roleIds = roleId ? [Number(roleId)] : [];

    if (!this.isEdit) {
      const payload: TenantsCreateModel = {
        firstName: p.firstName,
        lastName:  p.lastName,
        document:  String(p.document),
        phone:     p.phone,
        address:   p.address,
        cityId:    Number(loc.cityId),
        email:     acc.email,
        roleIds
      };
      this.dialogRef.close(payload);
      this.isLoading = false;
      return;
    }

    const t = this.data!.tenant!;
    const payload: TenantsUpdateModel = {
      id: t.id,
      firstName: p.firstName,
      lastName:  p.lastName,
      phone:     p.phone,
      address:   p.address,
      cityId:    Number(loc.cityId),
      email:     acc.email,
      roleIds,
      active:    this.form.get('active')?.value ?? t.active ?? true
    };

    this.dialogRef.close(payload);
    this.isLoading = false;
  }

  cancel(): void { this.dialogRef.close(null); }

  // ✔️ Usado en (blur) del input email
  fixEmail(): void {
    const emailCtrl = this.accountGroup.get('email');
    this.utils.coerceEmailTld(emailCtrl);
  }

  markAllTouched(): void {
    this.form.markAllAsTouched();
    Object.values(this.form.controls).forEach(c => c.updateValueAndValidity());
    Object.values(this.locationGroup.controls).forEach(c => c.updateValueAndValidity());
    Object.values(this.personGroup.controls).forEach(c => c.updateValueAndValidity());
    Object.values(this.accountGroup.controls).forEach(c => c.updateValueAndValidity());
  }

  getFirstError(control: AbstractControl | null, order: string[] = []): string | null {
    return this.errMsg.firstError(control, order);
  }
}

