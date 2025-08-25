import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output, inject } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  ValidatorFn,
  Validators
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { TextFieldModule } from '@angular/cdk/text-field';
import { map } from 'rxjs/operators';

import { DepartmentService } from '../../../features/setting/services/department/department.service';
import { DynamicFormField } from '../Models/Form/form.models';
import { FormType, formSchemas } from './dymanic-forms.config';

type Option = { value: string | number; label: string };

@Component({
  selector: 'app-dymanic-forms',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TextFieldModule,
    MatButtonModule,
    MatSelectModule,
    MatInputModule,
    MatFormFieldModule,
    MatCheckboxModule,
  ],
  templateUrl: './dymanic-forms.component.html',
  styleUrl: './dymanic-forms.component.css',
})
export class DymanicFormsComponent implements OnInit {
  @Input() formType!: FormType;
  @Input() initialData: any = {};
  @Input() selectOptions: Record<string, Option[]> = {};
  @Output() formSubmit = new EventEmitter<any>();

  form!: FormGroup;
  fields: DynamicFormField[] = [];

  private departmentService = inject(DepartmentService);
  constructor(private fb: FormBuilder) { }

  ngOnInit() {
    // 1) Clonar esquema
    this.fields = formSchemas[this.formType].map(f => ({ ...f }));

    // 2) Normaliza fechas a YYYY-MM-DD
    this.fields.forEach(field => {
      if (field.type === 'date') {
        const rawDate = this.initialData?.[field.name];
        if (rawDate) {
          const dateOnly = new Date(rawDate).toISOString().split('T')[0];
          this.initialData[field.name] = dateOnly;
        }
      }
    });

    // 3) Reglas especiales por tipo (ej. User edit)
    const isUser = this.formType === 'User';
    const isUserEdit = isUser && !!this.initialData?.id;
    if (isUserEdit) {
      const personField = this.fields.find(f => f.name === 'personId');
      if (personField) personField.type = 'hidden';
    }

    // 4) Cargar opciones dinámicas
    this.fields.forEach(field => {
      if (field.name === 'departmentId' && this.formType === 'City') {
        this.departmentService.getAll().pipe(
          map(depts => depts.map((d: any) => ({ value: d.id, label: d.name })))
        ).subscribe(options => this.setOptionsForField(field.name, options));
      } else if ((field.type === 'select' || (field as any).type === 'checkbox-list') && this.selectOptions[field.name]) {
        this.setOptionsForField(field.name, this.selectOptions[field.name]);
      }
    });

    // 5) Construir controles con VALIDADORES desde el schema
    const controls: Record<string, FormControl | FormGroup> = {};

    this.fields.forEach(field => {
      const init = this.initialData?.[field.name];
      const validators = this.mapValidators(field);

      switch (field.type) {
        case 'checkbox-list': {
          const fg = new FormGroup({});
          if (field.required || field.validations?.atLeastOne) fg.addValidators(atLeastOneTrueInGroupValidator);
          controls[field.name] = fg;
          break;
        }
        case 'checkbox': {
          const value = init ?? false;
          controls[field.name] = new FormControl<boolean>(!!value, { validators, updateOn: 'blur' });
          break;
        }
        default: {
          let value = init ?? null;
          if (field.type === 'select' && (field as any).multiple && value === null) value = [];
          controls[field.name] = new FormControl(value, { validators, updateOn: 'blur' });
        }
      }
    });

    this.form = new FormGroup(controls);

    // 6) Ajustes finales
    if (isUserEdit) {
      this.form.get('password')?.clearValidators();
      this.form.get('password')?.updateValueAndValidity();
      this.form.get('personId')?.clearValidators();
      this.form.get('personId')?.updateValueAndValidity();
    }

    if (this.formType === 'City' && this.initialData?.departmentId) {
      this.form.get('departmentId')?.setValue(this.initialData.departmentId);
    }

    if ((this.formType === 'Department' || this.formType === 'City') &&
        typeof this.initialData?.active === 'boolean') {
      this.form.get('active')?.setValue(this.initialData.active);
    }

    // Reconciliar checkbox-list
    this.fields.filter(f => (f as any).type === 'checkbox-list').forEach(f => {
      this.reconcileCheckboxListGroup(f.name);
    });
  }

  // ====== Mapeo de validadores ======
  private mapValidators(field: DynamicFormField): ValidatorFn[] {
    const v = field.validations ?? {};
    const out: ValidatorFn[] = [];

    // required
    if (field.required && field.type !== 'hidden') {
      if (field.type === 'checkbox') out.push(Validators.requiredTrue);
      else out.push(Validators.required);
    }

    // min/max length
    if (v.minLength) out.push(Validators.minLength(v.minLength));
    if (v.maxLength) out.push(Validators.maxLength(v.maxLength));

    // email
    if (field.type === 'email' || (v as any).email) out.push(Validators.email);

    // pattern (compilado con 'u')
    if (v.pattern) out.push(Validators.pattern(new RegExp(v.pattern, 'u')));

    // numérico con min/max (soporta coma)
    if (field.type === 'number' && (v.min !== undefined || v.max !== undefined)) {
      out.push(numberRange(v.min, v.max));
    }

    // onlySpaces para campos textuales
    const isTextual = ['text', 'textarea', 'email', 'password'].includes(field.type as any);
    if (v.onlySpaces || (field.required && isTextual)) {
      out.push((c: AbstractControl) => {
        const val = (c.value ?? '') as string;
        return typeof val === 'string' && val.trim().length === 0 ? { onlySpaces: true } : null;
      });
    }

    return out;
  }

  // ===== Helpers de opciones/checkbox-list =====
  private sortOptionsById(opts: Option[]) {
    return [...(opts ?? [])].sort((a, b) => Number(a.value) - Number(b.value));
  }
  private getField(name: string) { return this.fields.find(f => f.name === name); }
  private setOptionsForField(fieldName: string, options: Option[]) {
    const field = this.getField(fieldName);
    if (!field) return;
    (field as any).options = this.sortOptionsById(options ?? []);
    if ((field as any).type === 'checkbox-list' && this.form) this.reconcileCheckboxListGroup(fieldName);
  }

  private reconcileCheckboxListGroup(fieldName: string) {
    const field = this.getField(fieldName) as any;
    if (!field || field.type !== 'checkbox-list') return;

    const options = field.options ?? [];
    const group = this.form.get(fieldName) as FormGroup;
    if (!group) return;

    const selectedFromInit = new Set(
      Array.isArray(this.initialData?.[fieldName])
        ? this.initialData[fieldName].map(String)
        : []
    );

    for (const opt of options) {
      const key = String(opt.value);
      if (!group.contains(key)) {
        const initialChecked = selectedFromInit.has(key) || false;
        group.addControl(key, this.fb.control<boolean>(initialChecked));
      }
    }

    Object.keys(group.controls).forEach(ctrlKey => {
      const stillExists = options.some((o: any) => String(o.value) === ctrlKey);
      if (!stillExists) group.removeControl(ctrlKey);
    });

    group.updateValueAndValidity({ onlySelf: true, emitEvent: false });
  }

  trackByOption = (_: number, opt: Option) => String(opt.value);

  // ===== UX helpers =====
  compareById(o1: any, o2: any): boolean {
    if (o1 == null || o2 == null) return false;
    return String(o1) === String(o2);
  }

  onTrim(name: string) {
    const c = this.form.get(name);
    if (!c) return;
    const v = c.value;
    if (typeof v === 'string') {
      const t = v.trim().replace(/\s+/g, ' ');
      if (t !== v) c.setValue(t);
    }
  }

  blockNegative(e: KeyboardEvent) {
    if (e.key === '-' || e.key === 'Minus') e.preventDefault();
  }

  getError(name: string): string | null {
    const c = this.form.get(name);
    if (!c || !c.touched || !c.errors) return null;

    const f = this.getField(name);
    const label = f?.label ?? name;

    if (c.errors['required'])   return `${label} es obligatorio.`;
    if (c.errors['minlength'])  return `${label} requiere al menos ${c.errors['minlength'].requiredLength} caracteres.`;
    if (c.errors['maxlength'])  return `${label} no puede superar ${c.errors['maxlength'].requiredLength} caracteres.`;
    if (c.errors['email'])      return `Ingresa un correo válido (ej. usuario@dominio.com).`;
    if (c.errors['pattern'])    return `${label} tiene un formato inválido.`;
    if (c.errors['NaN'])        return `${label} debe ser numérico.`;
    if (c.errors['min'])        return `${label} no puede ser menor que ${c.errors['min'].min}.`;
    if (c.errors['max'])        return `${label} no puede ser mayor que ${c.errors['max'].max}.`;
    if (c.errors['onlySpaces']) return `${label} no puede ser solo espacios.`;
    return 'Valor inválido.';
  }

  // ===== Submit =====
  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    const result: any = { ...raw };

    for (const field of this.fields as any[]) {
      if (field.type === 'checkbox-list') {
        // Normaliza checkbox-list: FormGroup { [id]: boolean } -> number[] | string[]
        const fg = this.form.get(field.name) as FormGroup;
        const opts = field.options ?? [];
        const ids = opts
          .filter((o: any) => (fg.get(String(o.value)) as FormControl<boolean>)?.value === true)
          .map((o: any) => (typeof o.value === 'string' && !isNaN(+o.value) ? +o.value : o.value));
        result[field.name] = ids;

      } else if (field.type === 'number') {
        // convierte "100,50" => 100.5 / "" => null
        const v = result[field.name];
        result[field.name] = (v === null || v === '')
          ? null
          : Number(String(v).replace(',', '.'));

      } else if (typeof result[field.name] === 'string') {
        result[field.name] = result[field.name].trim().replace(/\s+/g, ' ');
      }
    }

    this.formSubmit.emit(result);
  }
}

/** Valida números (soporta coma) y chequea min/max si se proveen */
function numberRange(min?: number, max?: number) {
  return (c: AbstractControl) => {
    const raw = c.value;
    if (raw === null || raw === undefined || raw === '') return null;
    const n = Number(String(raw).replace(',', '.'));
    if (Number.isNaN(n)) return { NaN: true };
    if (min !== undefined && n < min) return { min: { min, actual: n } };
    if (max !== undefined && n > max) return { max: { max, actual: n } };
    return null;
  };
}

/** Validador: al menos un true dentro de un FormGroup<boolean> */
function atLeastOneTrueInGroupValidator(c: AbstractControl) {
  const v = c.value as Record<string, boolean>;
  return Object.values(v || {}).some(Boolean) ? null : { required: true };
}
