import { Component, inject, OnInit } from '@angular/core';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { MatDialog } from '@angular/material/dialog';
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { FormService } from '../../services/form/form.service';
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { FormSelectModel, FormUpdateModel } from '../../models/form.models';

@Component({
  selector: 'app-form',
  imports: [GenericTableComponent],
  templateUrl: './form.component.html',
  styleUrl: './form.component.css'
})
export class FormComponent implements OnInit {
  private readonly formService = inject(FormService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly sweetAlertService = inject(SweetAlertService);


  Forms: FormSelectModel[] = [];
  selectedForm: any = null;

  columns: TableColumn<FormSelectModel>[] = [];

  constructor(private dialog: MatDialog) { }

  ngOnInit(): void {
    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
      { key: 'name', header: 'Nombre' },
      { key: 'description', header: 'Descripción' },
      { key: 'route', header: 'Route' },
      { key: 'active', header: 'Active' }
    ];
    this.load();
  }

  load() {
    this.formService.getAll().subscribe({
      next: (data) => (this.Forms = data),
      error: (err) => console.error('Error al cargar formularios:', err)
    })
    // this.formService.getAllPruebas().subscribe({
    //   next: (data) => (this.Forms = data),
    //   error: (err) => console.error('Error al cargar formularios:', err)
    // })
  }

  onEdit(row: FormUpdateModel) {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: {
        entity: row,
        formType: 'Form'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        const id = row.id;
        this.formService.update(id, result).subscribe({
          next: () => {
            this.load();
            this.sweetAlertService.showNotification('Actualización Exitosa', 'Formulario actualizado exitosamente.', 'success');
          },
          error: err => {
            this.sweetAlertService.showNotification('Error', 'No se pudo actualizar el formulario.', 'error');
          }
        });
      }
    });
  }



  async onDelete(row: FormSelectModel) {
    const confirmed = await this.confirmDialog.confirm({
      title: 'Eliminar form',
      text: `¿Deseas eliminar el form "${row.name}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar',
    });

    if (confirmed) {
      this.formService.deleteLogic(row.id).subscribe({
        next: () => {
          this.sweetAlertService.showNotification('Eliminación Exitosa', 'Formulario eliminado exitosamente.', 'success');
          this.load();
        },
        error: err => {
          console.error('Error eliminando el formulario:', err);
          this.sweetAlertService.showNotification('Error', 'No se pudo eliminar el formulario.', 'error');
        }
      });
    }
  }

  onView(row: FormSelectModel) {
    console.log('Ver:', row);
  }

  onCreateNew() {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: {
        entity: {},
        formType: 'Form'
      }
    });

    dialogRef.afterClosed().subscribe((result: any) => {
      if (result) {
        this.formService.create(result).subscribe(res => {
          this.load();
          this.sweetAlertService.showNotification('Creación Exitosa', 'Formulario creado exitosamente.', 'success');
        }, err => {
          console.error('Error creando el formulario:', err);
          this.sweetAlertService.showNotification('Error', 'No se pudo crear el formulario.', 'error');
        });
      }
    });
  }

}
