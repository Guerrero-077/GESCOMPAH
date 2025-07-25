import { CommonModule } from '@angular/common';
import { Component, inject, model } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import {
  MatDialogTitle,
  MatDialogContent,
  MatDialogActions,
  MatDialogClose,
  MAT_DIALOG_DATA,
  MatDialogRef
} from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

import { LocalesModel } from '../../../../../shared/components/Models/card/card.models';
import { DynamicFormComponent } from '../../../../../shared/components/Form/dynamic-form-component/dynamic-form-component';
import { DynamicFormConfig } from '../../../../../shared/components/Models/Form/form.models';
import { buildLocalFormConfig } from '../../form-config/local-form.config';

@Component({
  selector: 'app-local-dialog-component',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatDialogClose,
    MatCheckboxModule,
    DynamicFormComponent
  ],
  templateUrl: './local-dialog-component.html',
  styleUrl: './local-dialog-component.css'
})
export class LocalDialogComponent {
  readonly dialogRef = inject(MatDialogRef<LocalDialogComponent>);
  readonly data = inject<LocalesModel>(MAT_DIALOG_DATA);
  readonly local = model({ ...this.data });

  selectedFiles: File[] = [];
  imagePreviews: string[] = [];

  // Formulario dinámico
  formConfig: DynamicFormConfig = buildLocalFormConfig(this.local());

  handleImageUpload(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;

    const files = Array.from(input.files);
    this.selectedFiles.push(...files);

    for (const file of files) {
      const reader = new FileReader();
      reader.onload = () => {
        const base64 = reader.result as string;
        this.imagePreviews.push(base64);
      };
      reader.readAsDataURL(file);
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  // Se ejecuta al enviar el formulario dinámico
  onFormSubmit(data: any): void {
    const result = {
      ...this.local(),
      ...data,
      files: this.selectedFiles
    };
    this.dialogRef.close(result);
  }
}
