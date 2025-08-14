import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { FormType, formSchemas } from './dymanic-forms.config';
import { DepartmentService } from '../../../features/setting/services/department/department.service';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-dymanic-forms',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatSlideToggleModule,
    MatButtonModule,
    MatSelectModule,
    MatInputModule,
    MatFormFieldModule
  ],
  templateUrl: './dymanic-forms.component.html',
  styleUrl: './dymanic-forms.component.css',
})
export class DymanicFormsComponent implements OnInit {
  @Input() formType!: FormType;
  @Input() initialData: any = {};
  @Input() selectOptions: Record<string, any[]> = {};

  @Output() formSubmit = new EventEmitter<any>();

  form!: FormGroup;
  fields: any[] = [];

  private departmentService = inject(DepartmentService);

  constructor(private fb: FormBuilder) { }

  ngOnInit() {
    console.log('DymanicFormsComponent - initialData:', this.initialData);
    console.log('DymanicFormsComponent - selectOptions:', this.selectOptions);

    // Clonar schema
    this.fields = formSchemas[this.formType].map(field => ({ ...field }));

    // ---- Reglas específicas por tipo ----
    const isUser = this.formType === 'User';
    const isUserEdit = isUser && !!this.initialData?.id;   // si viene id => edición

    // Para User EDIT: ocultar personId (no se cambia en update)
    if (isUserEdit) {
      const personField = this.fields.find(f => f.name === 'personId');
      if (personField) personField.type = 'hidden';
    }

    // Cargar opciones para selects
    this.fields.forEach(field => {
      if (field.name === 'departmentId' && this.formType === 'City') {
        this.departmentService.getAll().pipe(
          map(departments => departments.map((dep: any) => ({ value: dep.id, label: dep.name })))
        ).subscribe(options => { field.options = options; });
      } else if (field.type === 'select' && this.selectOptions[field.name]) {
        field.options = this.selectOptions[field.name];
      }
    });

    // Construcción de controles
    const formControls: Record<string, any> = {};
    this.fields.forEach(field => {
      // Valor inicial
      let initialValue = this.initialData?.[field.name];
      if (initialValue === undefined) {
        if (field.type === 'select' && field.multiple) initialValue = [];
        else if (field.type === 'checkbox') initialValue = false;
        else initialValue = null;
      }

      // Si el campo es hidden, NO forzamos required
      const validators = [];
      if (field.required && field.type !== 'hidden') {
        validators.push(Validators.required);
      }

      // Password: el schema lo pone required; si es edición, se quita más abajo
      formControls[field.name] = [initialValue, validators];
    });

    this.form = this.fb.group(formControls);

    // ---- Ajustes post-creación del form ----
    if (isUserEdit) {
      // Edición de User: password opcional
      this.form.get('password')?.clearValidators();
      this.form.get('password')?.updateValueAndValidity();

      // personId oculto y sin validaciones
      this.form.get('personId')?.clearValidators();
      this.form.get('personId')?.updateValueAndValidity();
    }

    // City: set departmentId si viene
    if (this.formType === 'City' && this.initialData?.departmentId) {
      this.form.get('departmentId')?.setValue(this.initialData.departmentId);
    }

    // Department / City: set active si viene
    if ((this.formType === 'Department' || this.formType === 'City')
      && typeof this.initialData?.active === 'boolean') {
      this.form.get('active')?.setValue(this.initialData.active);
    }

    console.log('DymanicFormsComponent - form.value after init:', this.form.value);
  }

  compareById(o1: any, o2: any): boolean {
    if (o1 == null || o2 == null) return false;
    return String(o1) === String(o2);
  }

  onSubmit() {
    if (this.form.valid) {
      this.formSubmit.emit(this.form.getRawValue()); // usar getRawValue por seguridad
    } else {
      this.form.markAllAsTouched();
    }
  }
}
