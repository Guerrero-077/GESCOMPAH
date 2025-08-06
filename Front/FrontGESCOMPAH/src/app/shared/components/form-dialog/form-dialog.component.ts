import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogActions, MatDialogModule, MatDialogRef, MatDialogTitle } from '@angular/material/dialog';
import { DymanicFormsComponent } from '../dymanic-forms/dymanic-forms.component';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-form-dialog',
  imports: [DymanicFormsComponent, MatDialogTitle, MatDialogActions, MatDialogModule, MatButtonModule ],
  templateUrl: './form-dialog.component.html',
  styleUrl: './form-dialog.component.css'
})
export class FormDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<FormDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  close(): void {
    this.dialogRef.close();
  }
}
