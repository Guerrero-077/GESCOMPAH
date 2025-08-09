import { Component } from '@angular/core';
import { CardComponent } from "../../../../shared/components/card/card.component";
import { ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatStepperModule } from '@angular/material/stepper';
import { MatSnackBar } from '@angular/material/snack-bar';
import { EstablishmentSelect } from '../../models/establishment.models';
import { EstablishmentService } from '../../services/establishment/establishment.service';
import { FormEstablishmentComponent } from '../form-establishment/form-establishment.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-establishments-list',
  imports: [
    CommonModule,
    CardComponent,
    ReactiveFormsModule,
    MatDialogModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatStepperModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './establishments-list.component.html',
  styleUrl: './establishments-list.component.css'
})
export class EstablishmentsListComponent {
  establishments: EstablishmentSelect[] = [];

  constructor(
    private snackbar: MatSnackBar,
    private dialog: MatDialog,
    private establishmentService: EstablishmentService
  ) { }

  ngOnInit(): void {
    this.loadLocales();
  }

  loadLocales(): void {
    this.establishmentService.getAll().subscribe({
      next: data => (this.establishments = data),
      error: err => this.snackbar.open('Error al cargar locales: ' + err.message, 'Cerrar')
    });
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(FormEstablishmentComponent, {
      width: '600px',
      data: null
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadLocales();
    });
  }

  openEditDialog(est: EstablishmentSelect): void {
    const dialogRef = this.dialog.open(FormEstablishmentComponent, {
      width: '600px',
      data: est
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadLocales();
    });
  }

  handleDelete(id: number): void {
    this.establishmentService.delete(id).subscribe({
      next: () => {
        this.snackbar.open('Local eliminado exitosamente', 'Cerrar', { duration: 2000 });
        this.loadLocales();
      },
      error: err => this.snackbar.open('Error al eliminar: ' + err.message, 'Cerrar')
    });
  }
}