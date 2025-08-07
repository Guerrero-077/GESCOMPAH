import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { FormDialogComponent } from '../../../../../shared/components/form-dialog/form-dialog.component';
import { GenericTableComponents } from "../../../../../shared/components/generic-table-components/generic-table-components";
import { TableColumn } from '../../../../../shared/models/TableColumn.models';
import { FormModule } from '../../../Models/form.model';
import { FormServices } from '../../../Services/Form/form-services';
import { ConfirmDialogService } from '../../../../../shared/Services/confirm-dialog-service';


@Component({
  selector: 'app-form-component',
  imports: [CommonModule, MatDialogModule, GenericTableComponents],
  templateUrl: './form-component.html',
  styleUrl: './form-component.css'
})

export class FormComponent implements OnInit {
  private readonly formService = inject(FormServices);
  private readonly confirmDialog = inject(ConfirmDialogService);

  Forms: FormModule[] = [];
  selectedForm: any = null;

  columns: TableColumn<FormModule>[] = [];

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
    this.formService.getAll("form").subscribe({
      next: (data) => (this.Forms = data),
      error: (err) => console.error('Error al cargar formularios:', err)
    })
    // this.formService.getAllPruebas().subscribe({
    //   next: (data) => (this.Forms = data),
    //   error: (err) => console.error('Error al cargar formularios:', err)
    // })
  }

  onEdit(row: FormModule) {
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
        this.formService.Update("form", id, result).subscribe({
          next: () => {
            console.log('Formulario actualizado correctamente');
            this.load();
          },
          error: err => console.error('Error actualizando el formulario:', err)
        });
      }
    });
  }



  async onDelete(row: FormModule) {
    const confirmed = await this.confirmDialog.confirm({
      title: 'Eliminar form',
      text: `¿Deseas eliminar el form "${row.name}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar',
    });

    if (confirmed) {
      this.formService.DeleteLogical('Form', row.id).subscribe({
        next: () => {
          console.log('Form eliminado correctamente');
          this.load();
        },
        error: err => console.error('Error eliminando el form:', err)
      });
    }
  }

  onView(row: FormModule) {
    console.log('Ver:', row);
  }

  onCreateNew() {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: {
        entity: {},
        formType: 'Form' // o 'User', 'Product', etc.
      }
    });

    dialogRef.afterClosed().subscribe((result: any) => {
      if (result) {
        this.formService.Add("form", result).subscribe(res => {
          this.load();
        });
      }
    });
  }

}
