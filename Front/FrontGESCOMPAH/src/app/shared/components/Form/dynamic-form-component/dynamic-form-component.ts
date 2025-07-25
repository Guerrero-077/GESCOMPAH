import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DynamicFormConfig, DynamicFormField } from '../../Models/Form/form.models';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dynamic-form-component',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule],
  templateUrl: './dynamic-form-component.html',
  styleUrl: './dynamic-form-component.css'
})
export class DynamicFormComponent implements OnInit {
  @Input() config!: DynamicFormConfig;
  @Output() formSubmit = new EventEmitter<any>();

  form!: FormGroup;

  constructor(private fb: FormBuilder) { }

  ngOnInit(): void {
    this.buildForm();
  }

  private buildForm() {
    const controls: { [key: string]: any } = {};

    this.config.fields.forEach((field: DynamicFormField) => {
      const validations = [];

      if (field.validations?.required) validations.push(Validators.required);
      if (field.validations?.minLength)
        validations.push(Validators.minLength(field.validations.minLength));
      if (field.validations?.maxLength)
        validations.push(Validators.maxLength(field.validations.maxLength));
      if (field.validations?.pattern)
        validations.push(Validators.pattern(field.validations.pattern));

      controls[field.name] = [field.value || '', validations];
    });

    this.form = this.fb.group(controls);
  }

  onSubmit(): void {
    if (this.form.valid) {
      this.formSubmit.emit(this.form.value);
    } else {
      this.form.markAllAsTouched();
    }
  }
}