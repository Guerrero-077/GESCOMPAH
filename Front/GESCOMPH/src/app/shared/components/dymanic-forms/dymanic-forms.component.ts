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
import { ThousandSeparatorDirective } from '../../directives/number/thousand-separator.directive';
import { DynamicFormField } from '../Models/Form/form.models';
import { FormType, formSchemas } from './dymanic-forms.config';
import { FormErrorComponent } from '../form-error/form-error.component';

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
    ThousandSeparatorDirective,
    FormErrorComponent,
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

    // 4) Cargar opciones dinÃ¡micas
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

    // Validación cruzada específica para Finance: effectiveTo >= effectiveFrom
    if (this.formType === 'Finance') {
      this.form.addValidators(financeDateRangeValidator('effectiveFrom', 'effectiveTo'));
    }

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

  // Mapeo de validadores
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

    // numÃ©rico con min/max (soporta coma)
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

  // Helpers de opciones/checkbox-list
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

  // Utilidades UX
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
      let next = t;
      // Normaliza 'key' en Finance a MAYÚSCULAS para evitar rechazos del backend
      if (this.formType === 'Finance' && name === 'key') {
        next = next.toUpperCase();
      }
      if (next !== v) c.setValue(next);
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
    if (c.errors['email'])      return `Ingresa un correo vÃ¡lido (ej. usuario@dominio.com).`;
    if (c.errors['pattern'])    return `${label} tiene un formato invÃ¡lido.`;
    if (c.errors['NaN'])        return `${label} debe ser numÃ©rico.`;
    if (c.errors['min'])        return `${label} no puede ser menor que ${c.errors['min'].min}.`;
    if (c.errors['max'])        return `${label} no puede ser mayor que ${c.errors['max'].max}.`;
    if (c.errors['onlySpaces']) return `${label} no puede ser solo espacios.`;
    return 'Valor invÃ¡lido.';
  }

  // Submit
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
        const v = result[field.name];
        if (v === null || v === '') {
          result[field.name] = null;
        } else {
          const sNum = String(v).replace(/\./g, '').replace(',', '.');
          // Backend de SystemParameter.Value es string: enviar como string en Finance
          if (this.formType === 'Finance' && field.name === 'value') {
            result[field.name] = sNum; // como cadena normalizada
          } else {
            result[field.name] = Number(sNum);
          }
        }

      }
      else if (field.type === 'date') {
        // Normaliza fecha
        // - Si viene vacía: null (para evitar enviar "")
        // - Si viene Date o string válida: YYYY-MM-DD (sin hora)
        const v = result[field.name];
        if (v === '' || v === undefined) {
          result[field.name] = null;
        } else if (v instanceof Date) {
          const d = v as Date;
          const yyyy = d.getFullYear();
          const mm = String(d.getMonth() + 1).padStart(2, '0');
          const dd = String(d.getDate()).padStart(2, '0');
          result[field.name] = `${yyyy}-${mm}-${dd}`;
        } else if (typeof v === 'string') {
          const d = new Date(v);
          if (!isNaN(d.getTime())) {
            const yyyy = d.getFullYear();
            const mm = String(d.getMonth() + 1).padStart(2, '0');
            const dd = String(d.getDate()).padStart(2, '0');
            result[field.name] = `${yyyy}-${mm}-${dd}`;
          } else if (!v) {
            result[field.name] = null;
          }
        }


      } else if (typeof result[field.name] === 'string') {
        result[field.name] = result[field.name].trim().replace(/\s+/g, ' ');
      }
    }

    this.formSubmit.emit(result);
  }
}

/** Valida nÃºmeros (soporta coma) y chequea min/max si se proveen */
function numberRange(min?: number, max?: number) {
  return (c: AbstractControl) => {
    const raw = c.value;
    if (raw === null || raw === undefined || raw === '') return null;
    // Soporta separadores de miles con punto y decimales con coma
    // Ej: "1.676.666" -> 1676666; "1.234,56" -> 1234.56
    const normalized = String(raw).replace(/\./g, '').replace(',', '.');
    const n = Number(normalized);
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

// Valida que end >= start (si ambos existen)
function financeDateRangeValidator(startKey: string, endKey: string) {
  return (group: AbstractControl) => {
    const g = group as FormGroup;
    const start = g.get(startKey)?.value;
    const end = g.get(endKey)?.value;
    if (!start || !end) return null; // alguno vacío: que otros validadores lo manejen
    const s = new Date(start);
    const e = new Date(end);
    if (isNaN(s.getTime()) || isNaN(e.getTime())) return null;
    return e.getTime() < s.getTime() ? { dateRange: true } : null;
  };
}
