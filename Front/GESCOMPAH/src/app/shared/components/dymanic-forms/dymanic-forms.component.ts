
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

    this.fields = formSchemas[this.formType].map(field => ({ ...field }));

    this.fields.forEach(field => {
      if (field.name === 'departmentId' && this.formType === 'City') {
        this.departmentService.getAll("Department").pipe(
          map(departments => departments.map(dep => ({ value: dep.id, label: dep.name })))
        ).subscribe(options => {
          field.options = options;
        });
      } else if (field.type === 'select' && this.selectOptions[field.name]) {
        field.options = this.selectOptions[field.name];
      }
    });

    const formControls: any = {};

    this.fields.forEach(field => {
      let initialValue = this.initialData?.[field.name];
      if (initialValue === undefined) {
        if (field.type === 'select' && field.multiple) {
          initialValue = [];
        } else if (field.type === 'checkbox') {
          initialValue = false;
        } else {
          initialValue = null;
        }
      }
      formControls[field.name] = [
        initialValue,
        field.required ? Validators.required : []
      ];
    });

    this.form = this.fb.group(formControls);

    if (this.formType === 'User' && this.initialData && this.initialData.id) {
      // This is an edit operation, make password not required
      this.form.get('password')?.clearValidators();
      this.form.get('password')?.updateValueAndValidity();
    }

    // Set initial value for departmentId if it exists in initialData
    if (this.formType === 'City' && this.initialData && this.initialData.departmentId) {
      this.form.get('departmentId')?.setValue(this.initialData.departmentId);
    }

    // For 'Department' form, set initial value for 'active' if it exists
    if (this.formType === 'Department' && this.initialData && typeof this.initialData.active === 'boolean') {
      this.form.get('active')?.setValue(this.initialData.active);
    }

    // For 'City' form, set initial value for 'active' if it exists
    if (this.formType === 'City' && this.initialData && typeof this.initialData.active === 'boolean') {
      this.form.get('active')?.setValue(this.initialData.active);
    }

    console.log('DymanicFormsComponent - form.value after init:', this.form.value);
  }

  compareById(o1: any, o2: any): boolean {
    console.log(`compareById - o1: ${o1} (type: ${typeof o1}), o2: ${o2} (type: ${typeof o2})`);
    if (o1 == null || o2 == null) return false;
    return String(o1) === String(o2);
  }


  onSubmit() {
    if (this.form.valid) {
      this.formSubmit.emit(this.form.value);
    } else {
      this.form.markAllAsTouched();
    }
  }
}
