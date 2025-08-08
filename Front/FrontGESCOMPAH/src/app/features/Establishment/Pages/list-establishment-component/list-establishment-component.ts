import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';


import { LocalCard } from "../../../../shared/components/card/local-card/local-card";
import { EstablishmentSelect } from '../../Models/Establishment.models';
import { LocalesService } from '../../Services/Locales/locales-service';
import { EstablishmentFormComponent } from '../Form/establishment-form-component/establishment-form-component';

@Component({
  selector: 'app-list-establishment-component',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatDialogModule,
    LocalCard,
  ],
  templateUrl: './list-establishment-component.html',
  styleUrl: './list-establishment-component.css'
})
export class ListEstablishmentComponent {
  locales: EstablishmentSelect[] = [];
  snackBar: any;

  constructor(
    private snackbar: MatSnackBar,
    private dialog: MatDialog,
    private localesService: LocalesService
  ) { }

  ngOnInit(): void { this.loadLocales(); }

  loadLocales(): void {
    this.localesService.getAll().subscribe({
      next: data => (this.locales = data),
      error: err => this.snackBar.open('Error al cargar locales: ' + err.message, 'Cerrar')
    });
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(EstablishmentFormComponent, {
      width: '600px',
      data: null
    });

    dialogRef.afterClosed().subscribe(result => { if (result) this.loadLocales(); });
  }

  openEditDialog(est: EstablishmentSelect): void {
    const dialogRef = this.dialog.open(EstablishmentFormComponent, {
      width: '600px',
      data: est            // <-- pasamos el registro que se va a editar
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) this.loadLocales();   // → recargar la lista
    });
  }

  handleDelete(id: number): void {
    this.localesService.delete(id).subscribe({
      next: () => {
        this.snackBar.open('Local eliminado exitosamente', 'Cerrar', { duration: 2000 });
        this.loadLocales();
      },
      error: err => this.snackBar.open('Error al eliminar: ' + err.message, 'Cerrar')
    });
  }

}
