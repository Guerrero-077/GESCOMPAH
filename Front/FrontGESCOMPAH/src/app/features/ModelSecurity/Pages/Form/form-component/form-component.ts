import { Component, inject, OnInit } from '@angular/core';
import { GenericTableComponents } from "../../../../../shared/components/generic-table-components/generic-table-components";
import { TableColumn } from '../../../../../shared/models/TableColumn.models';
import { FormModule } from '../../../Models/form.model';
import { FormServices } from '../../../Services/Form/form-services';
import { DymanicFormsComponent } from "../../../../../shared/components/dymanic-forms/dymanic-forms.component";

@Component({
  selector: 'app-form-component',
  imports: [GenericTableComponents, DymanicFormsComponent],
  templateUrl: './form-component.html',
  styleUrl: './form-component.css'
})
export class FormComponent implements OnInit {
  private readonly formService = inject(FormServices);
  Forms: FormModule[] = [];

  columns: TableColumn<FormModule>[] = [];

  ngOnInit(): void {
    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
      { key: 'name', header: 'Nombre' },
      { key: 'description', header: 'Descripción' }
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
    console.log('Editar:', row);
  }

  onDelete(row: FormModule) {
    console.log('Eliminar:', row);
  }

  onView(row: FormModule) {
    console.log('Ver:', row);
  }




}
