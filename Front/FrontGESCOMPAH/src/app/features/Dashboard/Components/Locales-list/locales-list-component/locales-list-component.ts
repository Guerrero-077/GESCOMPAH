import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LocalcardComponent } from '../../../../../shared/components/card/localcard-component/localcard-component';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { LocalDialogComponent } from '../../Dialogo/local-dialog-component/local-dialog-component';
import { LocalCreateModel, LocalesModel } from '../../../../../shared/components/Models/card/card.models';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { LocalesService } from '../../../../../Core/Services/Locales/locales-service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-locales-list',
  standalone: true,
  imports: [
    CommonModule,
    LocalcardComponent,
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    MatButtonModule,
    MatDialogModule,
    MatSnackBarModule
  ],
  templateUrl: './locales-list-component.html',
  styleUrls: ['./locales-list-component.css'],
})
export class LocalesListComponent implements OnInit {
  private readonly localesService = inject(LocalesService);
  private readonly dialog = inject(MatDialog);
  private readonly snackbar = inject(MatSnackBar);

  locales: LocalesModel[] = [];

  ngOnInit(): void {
    this.loadLocales();
  }

  private loadLocales(): void {
    this.localesService.getLocales().subscribe({
      next: (data) => this.locales = data,
      error: () => this.snackbar.open('Error al cargar los locales', 'Cerrar', { duration: 3000 })
    });
  }

  handleView(localId: number): void {
    console.log('Ver local:', localId);
  }

  handleCreate(): void {
    const dialogRef = this.dialog.open<LocalDialogComponent, null, LocalCreateModel>(
      LocalDialogComponent,
      { data: null }
    );

    dialogRef.afterClosed().subscribe((result: LocalCreateModel | undefined) => {
      if (result) {
        this.localesService.createLocal(result).subscribe({
          next: (newLocal) => {
            this.locales.push(newLocal); // Inserta directamente
            this.snackbar.open('Local creado correctamente', 'Cerrar', { duration: 3000 });
          },
          error: () => this.snackbar.open('Error al crear el local', 'Cerrar', { duration: 3000 })
        });
      }
    });
  }

  handleEdit(localId: number): void {
    console.log('Editar local:', localId);
  }


  handleDelete(id: number): void {
    Swal.fire({
      title: '¿Estás seguro?',
      text: 'Esta acción eliminará el local. ¿Deseas continuar?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Sí, eliminar',
      cancelButtonText: 'Cancelar',
      reverseButtons: true
    }).then((result) => {
      if (result.isConfirmed) {
        this.localesService.deleteLocal(id, false).subscribe({
          next: () => {
            Swal.fire('Eliminado', 'El local fue eliminado exitosamente', 'success');
            this.loadLocales(); // Refresca la lista
          },
          error: (err) => {
            Swal.fire('Error', err?.message || 'No se pudo eliminar el local', 'error');
          }
        });
      }
    });
  }


}
