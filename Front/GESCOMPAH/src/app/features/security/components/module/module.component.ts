import { Component, inject, OnInit } from '@angular/core';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { MatDialog } from '@angular/material/dialog';
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { ModulesModule } from '../../models/module.models';
import { ModuleService } from '../../services/module/module.service';
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';

@Component({
  selector: 'app-module',
  imports: [GenericTableComponent],
  templateUrl: './module.component.html',
  styleUrl: './module.component.css'
})
export class ModuleComponent implements OnInit {
  private readonly moduleService = inject(ModuleService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly sweetAllertService = inject(SweetAlertService);

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
            this.sweetAllertService.showNotification('Actualización Exitosa', 'Módulo actualizado exitosamente.', 'success');
          },
          error: err => {
            console.error('Error actualizando el módulo:', err)
            this.sweetAllertService.showNotification('Error', 'No se pudo actualizar el módulo.', 'error');
          }
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
          this.sweetAllertService.showNotification('Eliminación Exitosa', 'Módulo eliminado exitosamente.', 'success');
        },
        error: err => {
          console.error('Error eliminando el módulo:', err)
          this.sweetAllertService.showNotification('Error', 'No se pudo eliminar el módulo.', 'error');
        }
      });
    }
  }

  onCreateNew() {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: {
        entity: {},
        formType: 'Module'
      }
    });

    dialogRef.afterClosed().subscribe((result: any) => {
      if (result) {
        this.moduleService.Add("Module", result).subscribe(res => {
          this.load();
          this.sweetAllertService.showNotification('Creación Exitosa', 'Módulo creado exitosamente.', 'success');
        }, err => {
          console.error('Error creando el módulo:', err);
          this.sweetAllertService.showNotification('Error', 'No se pudo crear el módulo.', 'error');
        });
      }
    });
  }


  onView(row: ModulesModule) {
    console.log('Ver:', row);
  }
}
