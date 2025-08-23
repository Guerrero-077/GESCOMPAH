import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output, inject } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { TextFieldModule } from '@angular/cdk/text-field';
import { map } from 'rxjs/operators';

import { FormType, formSchemas, DynamicFormField } from './dymanic-forms.config';
import { DepartmentService } from '../../../features/setting/services/department/department.service';

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

    // 2) Formatear campos tipo date antes de inicializar el formulario
    this.fields.forEach(field => {
      if (field.type === 'date') {
        const rawDate = this.initialData?.[field.name];
        if (rawDate) {
          const dateOnly = new Date(rawDate).toISOString().split('T')[0];
          this.initialData[field.name] = dateOnly;
        }
      }
    });

    // 3) Reglas especiales por tipo
    const isUser = this.formType === 'User';
    const isUserEdit = isUser && !!this.initialData?.id;

    if (isUserEdit) {
      const personField = this.fields.find(f => f.name === 'personId');
      if (personField) personField.type = 'hidden';
    }

    // 4) Cargar opciones dinámicas (selects, checkbox-list)
    this.fields.forEach(field => {
      if (field.name === 'departmentId' && this.formType === 'City') {
        this.departmentService.getAll().pipe(
          map(depts => depts.map((d: any) => ({ value: d.id, label: d.name })))
        ).subscribe(options => this.setOptionsForField(field.name, options));
      } else if ((field.type === 'select' || field.type === 'checkbox-list') && this.selectOptions[field.name]) {
        this.setOptionsForField(field.name, this.selectOptions[field.name]);
      }
    });

    // 5) Construir controles del formulario
    const controls: Record<string, any> = {};

    this.fields.forEach(field => {
      const init = this.initialData?.[field.name];

      switch (field.type) {
        case 'checkbox-list': {
          const fg = new FormGroup({});
          if (field.required) fg.addValidators(atLeastOneTrueInGroupValidator);
          controls[field.name] = fg;
          break;
        }
        default: {
          let value = init;
          if (value === undefined) {
            if (field.type === 'select' && field.multiple) value = [];
            else if (field.type === 'checkbox') value = false;
            else value = null;
          }
          const validators = [];
          if (field.required && field.type !== 'hidden') {
            validators.push(field.type === 'checkbox' ? Validators.requiredTrue : Validators.required);
          }
          controls[field.name] = [value, validators];
          break;
        }
      }
    });

    this.form = this.fb.group(controls);

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
    this.fields.filter(f => f.type === 'checkbox-list').forEach(f => {
      this.reconcileCheckboxListGroup(f.name);
    });
  }

  // ===== Helpers de opciones/checkbox-list =====

  private sortOptionsById(opts: Option[]) {
    return [...(opts ?? [])].sort((a, b) => Number(a.value) - Number(b.value));
  }

  private getField(name: string) {
    return this.fields.find(f => f.name === name);
  }

  private setOptionsForField(fieldName: string, options: Option[]) {
    const field = this.getField(fieldName);
    if (!field) return;
    field.options = this.sortOptionsById(options ?? []);
    if (field.type === 'checkbox-list' && this.form) {
      this.reconcileCheckboxListGroup(fieldName);
    }
  }

  private reconcileCheckboxListGroup(fieldName: string) {
    const field = this.getField(fieldName);
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
      const stillExists = options.some(o => String(o.value) === ctrlKey);
      if (!stillExists) group.removeControl(ctrlKey);
    });

    group.updateValueAndValidity({ onlySelf: true, emitEvent: false });
  }

  trackByOption = (_: number, opt: Option) => String(opt.value);

  // ===== Utils =====

  compareById(o1: any, o2: any): boolean {
    if (o1 == null || o2 == null) return false;
    return String(o1) === String(o2);
  }

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    const result: any = { ...raw };

    // Normaliza checkbox-list: FormGroup { [id]: boolean } -> number[]
    for (const field of this.fields) {
      if (field.type === 'checkbox-list') {
        const fg = this.form.get(field.name) as FormGroup;
        const opts = field.options ?? [];
        const ids = opts
          .filter(o => (fg.get(String(o.value)) as FormControl<boolean>)?.value === true)
          .map(o => (typeof o.value === 'string' && !isNaN(+o.value) ? +o.value : o.value));
        result[field.name] = ids;
      }
    }

    this.formSubmit.emit(result);
  }
}

/** Validador: al menos un true dentro de un FormGroup<boolean> */
function atLeastOneTrueInGroupValidator(c: AbstractControl) {
  const v = c.value as Record<string, boolean>;
  return Object.values(v || {}).some(Boolean) ? null : { required: true };
}
