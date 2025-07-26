import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DynamicFormComponent } from '../../Form/dynamic-form-component/dynamic-form-component';
import { GenericFormDialogData } from '../../Models/Modal/modal.models';

@Component({
  selector: 'app-modal-componenet',
  imports: [
    CommonModule,
    FormsModule,
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatButtonModule,
    MatDialogClose,
    DynamicFormComponent
  ],
  templateUrl: './modal-componenet.html',
  styleUrl: './modal-componenet.css'
})
export class ModalComponenet {
  readonly dialogRef = inject(MatDialogRef<ModalComponenet>);
  readonly data = inject<GenericFormDialogData>(MAT_DIALOG_DATA);

  selectedFiles: File[] = [];
  imagePreviews: string[] = [];

  handleImageUpload(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files) return;
    const files = Array.from(input.files);
    this.selectedFiles.push(...files);

    for (const file of files) {
      const reader = new FileReader();
      reader.onload = () => {
        this.imagePreviews.push(reader.result as string);
      };
      reader.readAsDataURL(file);
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onFormSubmit(formData: any): void {
    const result = {
      ...this.data.model,
      ...formData,
      files: this.selectedFiles
    };
    this.dialogRef.close(result);
  }
}