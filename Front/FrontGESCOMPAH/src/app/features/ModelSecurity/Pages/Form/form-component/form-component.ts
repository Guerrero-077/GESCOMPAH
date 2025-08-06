import { Component, inject, OnInit } from '@angular/core';
import { GenericTableComponents } from "../../../../../shared/components/generic-table-components/generic-table-components";
import { TableColumn } from '../../../../../shared/models/TableColumn.models';
import { FormModule } from '../../../Models/form.model';
import { FormServices } from '../../../Services/Form/form-services';
import { DymanicFormsComponent } from "../../../../../shared/components/dymanic-forms/dymanic-forms.component";

declare var bootstrap: any;

@Component({
  selector: 'app-form-component',
  imports: [GenericTableComponents, DymanicFormsComponent],
  templateUrl: './form-component.html',
  styleUrl: './form-component.css'
})
export class FormComponent implements OnInit {
  private readonly formService = inject(FormServices);
  Forms: FormModule[] = [];
  selectedForm: any = null;

  columns: TableColumn<FormModule>[] = [];

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
    this.selectedForm = row;
    console.log(this.selectedForm)
    const modal = new bootstrap.Modal(document.getElementById('editFormModal')!);
    modal.show();
  }

  onDelete(row: FormModule) {
    console.log('Eliminar:', row);
  }

  onView(row: FormModule) {
    console.log('Ver:', row);
  }




}
