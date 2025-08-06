import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { FormType, formSchemas } from './dymanic-forms.config';
import { CommonModule } from '@angular/common';
import {MatSlideToggleModule} from '@angular/material/slide-toggle';

@Component({
  selector: 'app-dymanic-forms',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatSlideToggleModule],
  templateUrl: './dymanic-forms.component.html',
  styleUrl: './dymanic-forms.component.css'
})
export class DymanicFormsComponent  implements OnInit {
  @Input() formType!: FormType;
  @Input() initialData: any = {};
  @Input() selectOptions: Record<string, any[]> = {};

  form!: FormGroup;
  fields: any[] = [];

  constructor(private fb: FormBuilder) {}

  ngOnInit() {
    this.fields = formSchemas[this.formType].map(field => ({ ...field }));

    this.fields.forEach(field => {
      if (field.type === 'select' && this.selectOptions[field.name]) {
        field.options = this.selectOptions[field.name];
      }
    });

    const formControls: any = {};

    this.fields.forEach(field => {
      formControls[field.name] = [
        this.initialData?.[field.name] || '',
        field.required ? Validators.required : []
      ];
    });

    this.form = this.fb.group(formControls);

  }
  
  onSubmit() {
    if (this.form.valid) {
      console.log('Formulario enviado:', this.form.value);
    } else {
      this.form.markAllAsTouched();
    }
  }
}
