import { Component, inject, OnInit } from '@angular/core';
import { GenericTableComponents } from "../../../../../shared/components/generic-table-components/generic-table-components";
import { TableColumn } from '../../../../../shared/models/TableColumn.models';
import { FormModule } from '../../../Models/form.model';
import { FormServices } from '../../../Services/Form/form-services';
import { DymanicFormsComponent } from "../../../../../shared/components/dymanic-forms/dymanic-forms.component";
import { CommonModule } from '@angular/common'; 
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { FormDialogComponent } from '../../../../../shared/components/form-dialog/form-dialog.component';


@Component({
  selector: 'app-form-component',
  imports: [GenericTableComponents, DymanicFormsComponent, CommonModule, MatDialogModule],
  templateUrl: './form-component.html',
  styleUrl: './form-component.css'
})
export class FormComponent implements OnInit {
  private readonly formService = inject(FormServices);
  Forms: FormModule[] = [];
  selectedForm: any = null;

  columns: TableColumn<FormModule>[] = [];

  constructor(private dialog: MatDialog) {}

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
    // this.formService.getAll("form").subscribe({
    //   next: (data) => (this.Forms = data),
    //   error: (err) => console.error('Error al cargar formularios:', err)
    // })
    this.formService.getAllPruebas().subscribe({
      next: (data) => (this.Forms = data),
      error: (err) => console.error('Error al cargar formularios:', err)
    })
  }

  onEdit(row: FormModule) {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: {
        entity: row, // los datos a editar
        formType: 'Form' // o 'User', 'Product', etc.
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      this.selectedForm = null;
      if (result) { //comentario
        this.load(); // Recarga si es necesario
      }
    });
  }

  onDelete(row: FormModule) {
    console.log('Eliminar:', row);
  }

  onView(row: FormModule) {
    console.log('Ver:', row);
  }

}
