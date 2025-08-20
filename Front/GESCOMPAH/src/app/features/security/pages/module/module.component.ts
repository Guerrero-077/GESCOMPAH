import { AfterViewInit, ChangeDetectorRef, Component, inject, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { MatDialog } from '@angular/material/dialog';
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { ModuleService } from '../../services/module/module.service';
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { ModuleSelectModel, ModuleUpdateModel } from '../../models/module.models';
import { ModuleStore } from '../../services/module/module.store';
import { CommonModule } from '@angular/common';
import { ToggleButtonComponent } from "../../../../shared/components/toggle-button-component/toggle-button-component.component";

@Component({
  selector: 'app-module',
  imports: [GenericTableComponent, CommonModule, ToggleButtonComponent],
  templateUrl: './module.component.html',
  styleUrl: './module.component.css'
})
export class ModuleComponent implements OnInit {
  private readonly moduleStore = inject(ModuleStore);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly sweetAllertService = inject(SweetAlertService);

  modules$ = this.moduleStore.modules$;

  columns: TableColumn<ModuleSelectModel>[] = [];
  selectedForm: any = null;

  // 👇 Capturamos el template del HTML
  @ViewChild('estadoTemplate', { static: true }) estadoTemplate!: TemplateRef<any>;

  constructor(private dialog: MatDialog, private cdr: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
      { key: 'name', header: 'Nombre' },
      { key: 'description', header: 'Descripción' },
      { key: 'route', header: 'Route' },
      {
        key: 'active',
        header: 'Estado',
        type: 'custom',
        template: this.estadoTemplate
      }
    ];
  }

  onEdit(row: ModuleUpdateModel) {
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
        this.moduleStore.update(id, result).subscribe({
          next: () => {
            console.log('Módulo actualizado correctamente');
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

  async onDelete(row: ModuleSelectModel) {
    const confirmed = await this.confirmDialog.confirm({
      title: 'Eliminar módulo',
      text: `¿Deseas eliminar el módulo "${row.name}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar',
    });

    if (confirmed) {
      this.moduleStore.deleteLogic(row.id).subscribe({
        next: () => {
          console.log('Módulo eliminado correctamente');
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
        this.moduleStore.create(result).subscribe(res => {
          this.sweetAllertService.showNotification('Creación Exitosa', 'Módulo creado exitosamente.', 'success');
        }, err => {
          console.error('Error creando el módulo:', err);
          this.sweetAllertService.showNotification('Error', 'No se pudo crear el módulo.', 'error');
        });
      }
    });
  }


  onView(row: ModuleSelectModel) {
    console.log('Ver:', row);
  }

  // ----- Toggle estado (activo/inactivo) -----
  onToggleActive(row: ModuleSelectModel, e: { checked: boolean }) {
    const previous = row.active;
    row.active = e.checked;
    this.moduleStore.changeActiveStatus(row.id, e.checked).subscribe({
      next: (updated) => {
        // sincronizar con lo que devuelve el backend
        row.active = updated.active ?? row.active;
        this.sweetAllertService.showNotification(
          'Éxito',
          `Modulo ${row.active ? 'activado' : 'desactivado'} correctamente.`,
          'success'
        );
      },
      error: (err) => {
        // revertir si falla
        row.active = previous;
        this.sweetAllertService.showNotification(
          'Error',
          err?.error?.detail || 'No se pudo cambiar el estado.',
          'error'
        );
      }
    });
  }
}
