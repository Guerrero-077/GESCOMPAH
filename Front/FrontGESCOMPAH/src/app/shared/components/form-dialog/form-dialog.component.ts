import { Component, Inject, Input } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogActions, MatDialogModule, MatDialogRef, MatDialogTitle } from '@angular/material/dialog';
import { DymanicFormsComponent } from '../dymanic-forms/dymanic-forms.component';
import { MatButtonModule } from '@angular/material/button';
import { FormType } from '../dymanic-forms/dymanic-forms.config';

@Component({
  selector: 'app-form-dialog',
  imports: [DymanicFormsComponent, MatDialogTitle, MatDialogActions, MatDialogModule, MatButtonModule ],
  templateUrl: './form-dialog.component.html',
  styleUrl: './form-dialog.component.css'
})
export class FormDialogComponent {
  entity!: any;
  formType!: FormType;

  constructor(
    public dialogRef: MatDialogRef<FormDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { entity: any; formType: FormType }
  ) {
    this.entity = data.entity;
    this.formType = data.formType;
  }

  close(data: any): void {
    this.dialogRef.close(data);
  }
}


