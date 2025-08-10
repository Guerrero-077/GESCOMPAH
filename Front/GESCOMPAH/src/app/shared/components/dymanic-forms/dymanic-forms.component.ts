
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
      formControls[field.name] = [
        this.initialData?.[field.name] ?? '',
        field.required ? Validators.required : []
      ];
    });

    this.form = this.fb.group(formControls);

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
  }

  onSubmit() {
    if (this.form.valid) {
      this.formSubmit.emit(this.form.value);
    } else {
      this.form.markAllAsTouched();
    }
  }
}
