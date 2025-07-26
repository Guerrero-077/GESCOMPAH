import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import Swal from 'sweetalert2';

import { LocalCard } from '../../../../shared/components/card/local-card/local-card';
import { Header } from '../../../../shared/components/Header/header/header';
import { ModalComponenet } from '../../../../shared/components/Modal/modal-componenet/modal-componenet';
import {
  LocalesModel,
  LocalCreateModel,
  LocalUpdateModel
} from '../../../../shared/components/Models/card/card.models';
import { GenericFormDialogData } from '../../../../shared/components/Models/Modal/modal.models';
import { buildLocalFormConfig } from '../../Models/form-config/local-form.config';
import { LocalesService } from '../../Services/Locales/locales-service';

@Component({
  selector: 'app-list-establishment-component',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDialogModule,
    MatSnackBarModule,
    Header,
    LocalCard
  ],
  templateUrl: './list-establishment-component.html',
  styleUrl: './list-establishment-component.css'
})
export class ListEstablishmentComponent implements OnInit {
  private readonly localesService = inject(LocalesService);
  private readonly dialog = inject(MatDialog);
  private readonly snackbar = inject(MatSnackBar);
  readonly router = inject(Router);

  locales: LocalesModel[] = [];

  ngOnInit(): void {
    this.loadLocales();
  }

  private loadLocales(): void {
    this.localesService.getLocales().subscribe({
      next: (data) => (this.locales = data),
      error: () => {
        this.snackbar.open('Error al cargar los locales', 'Cerrar', { duration: 3000 });
      }
    });
  }

  handleView(localId: number): void {
    console.log('Ver local:', localId);
    // Navegación o apertura de modal si aplica
  }

  handleCreate(): void {
    const dialogRef = this.dialog.open<ModalComponenet, GenericFormDialogData, LocalCreateModel>(
      ModalComponenet,
      {
        data: {
          title: 'Crear nuevo local',
          subtitle: 'Información básica del local',
          formConfig: buildLocalFormConfig(),
          allowImageUpload: true
        }
      }
    );

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.localesService.createLocal(result).subscribe({
          next: (newLocal) => {
            this.locales.push(newLocal);
            this.snackbar.open('Local creado correctamente', 'Cerrar', { duration: 3000 });
          },
          error: () => {
            this.snackbar.open('Error al crear el local', 'Cerrar', { duration: 3000 });
          }
        });
      }
    });
  }

  handleEdit(local: LocalesModel): void {
    const editable: LocalUpdateModel = {
      id: local.id,
      name: local.name,
      description: local.description,
      areaM2: local.areaM2,
      rentValueBase: local.rentValueBase,
      files: []
    };

    const dialogRef = this.dialog.open<ModalComponenet, GenericFormDialogData, LocalUpdateModel>(
      ModalComponenet,
      {
        data: {
          title: 'Editar local',
          subtitle: 'Modificar información del local',
          formConfig: buildLocalFormConfig(editable),
          allowImageUpload: true
        }
      }
    );

    dialogRef.afterClosed().subscribe((result) => {
      if (!result) return;

      if (!result.id) {
        this.snackbar.open('Error: ID no presente en la edición', 'Cerrar', { duration: 3000 });
        return;
      }

      this.localesService.updateLocal(result).subscribe({
        next: (updated) => {
          const index = this.locales.findIndex((l) => l.id === updated.id);
          if (index !== -1) {
            this.locales[index] = updated;
          }
          this.snackbar.open('Local actualizado correctamente', 'Cerrar', { duration: 3000 });
        },
        error: () => {
          this.snackbar.open('Error al actualizar el local', 'Cerrar', { duration: 3000 });
        }
      });
    });
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
            this.loadLocales();
          },
          error: (err) => {
            Swal.fire('Error', err?.message || 'No se pudo eliminar el local', 'error');
          }
        });
      }
    });
  }
}
