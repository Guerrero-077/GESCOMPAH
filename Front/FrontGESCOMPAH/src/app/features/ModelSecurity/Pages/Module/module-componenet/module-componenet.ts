import { Component, inject, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormDialogComponent } from '../../../../../shared/components/form-dialog/form-dialog.component';
import { GenericTableComponents } from '../../../../../shared/components/generic-table-components/generic-table-components';
import { TableColumn } from '../../../../../shared/models/TableColumn.models';
import { ConfirmDialogService } from '../../../../../shared/Services/confirm-dialog-service';
import { ModulesModule } from '../../../Models/module.models';
import { ModuleServices } from '../../../Services/Module/module-services';

@Component({
  selector: 'app-module-componenet',
  standalone: true,
  imports: [GenericTableComponents],
  templateUrl: './module-componenet.html',
  styleUrls: ['./module-componenet.css'] // ❗ corregido: `styleUrl` → `styleUrls`
})
export class ModuleComponenet implements OnInit {
  private readonly moduleService = inject(ModuleServices);
  private readonly confirmDialog = inject(ConfirmDialogService);

  Forms: ModulesModule[] = [];

  columns: TableColumn<ModulesModule>[] = [];
  selectedForm: any = null;

  constructor(private dialog: MatDialog) { }

  ngOnInit(): void {
    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
      { key: 'name', header: 'Nombre' },
      { key: 'description', header: 'Descripción' }
    ];
    this.load();
  }

  load() {
    this.moduleService.getAll('module').subscribe({
      next: (data) => (this.Forms = data),
      error: (err) => console.error('Error al cargar módulos:', err)
    });
  }

  onEdit(row: ModulesModule) {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: {
        entity: row,
        formType: 'Module'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        const id = row.id;
        this.moduleService.Update('module', id, result).subscribe({
          next: () => {
            console.log('Módulo actualizado correctamente');
            this.load();
          },
          error: err => console.error('Error actualizando el módulo:', err)
        });
      }
    });
  }

  async onDelete(row: ModulesModule) {
    const confirmed = await this.confirmDialog.confirm({
      title: 'Eliminar módulo',
      text: `¿Deseas eliminar el módulo "${row.name}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar',
    });

    if (confirmed) {
      this.moduleService.DeleteLogical('Module', row.id).subscribe({
        next: () => {
          console.log('Módulo eliminado correctamente');
          this.load();
        },
        error: err => console.error('Error eliminando el módulo:', err)
      });
    }
  }

  onView(row: ModulesModule) {
    console.log('Ver:', row);
  }
}
