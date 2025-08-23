import { Component, TemplateRef, ViewChild } from '@angular/core';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { ToggleButtonComponent } from "../../../../shared/components/toggle-button-component/toggle-button-component.component";
import { MatDialog } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { FinanceSelectModels } from '../../models/finance.models';
import { CommonModule } from '@angular/common';
import { FinanceStore } from '../../services/finance/finance.store';

@Component({
  selector: 'app-finance',
  imports: [GenericTableComponent, ToggleButtonComponent, CommonModule],
  templateUrl: './finance.component.html',
  styleUrl: './finance.component.css'
})
export class FinanceComponent {
  finances$: Observable<FinanceSelectModels[]>;

  @ViewChild('estadoTemplate', { static: true }) estadoTemplate!: TemplateRef<any>;

  columns: TableColumn<FinanceSelectModels>[] = [
    { key: 'id', header: 'ID' },
    { key: 'key', header: 'Nombre' },
    { key: 'value', header: 'valor' },
    { key: 'effectiveFrom', header: 'Vigeste desde' },
    { key: 'effectiveTo', header: 'Vigente Hasta' },
  ];

  constructor(
    private store: FinanceStore,
    private dialog: MatDialog,
    private sweetAlert: SweetAlertService
  ) {
    this.finances$ = this.store.finances$;
  }

  ngOnInit(): void {
    this.columns = [
      ...this.columns,
      {
        key: 'active',
        header: 'Estado',
        type: 'custom',
        template: this.estadoTemplate
      }
    ];
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '400px',
      data: {
        entity: {},
        formType: 'Finance'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.store.create(result).subscribe({
          next: () => {
            this.sweetAlert.showNotification('Éxito', 'Ciudad creada exitosamente', 'success');
          }
        });
      }
    });
  }

  openEditDialog(row: FinanceSelectModels): void {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '400px',
      data: {
        entity: row,
        formType: 'Finance'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.store.update(result).subscribe({
          next: () => {
            this.sweetAlert.showNotification('Éxito', 'Ciudad actualizada exitosamente', 'success');
          }
        });
      }
    });
  }

  handleDelete(row: FinanceSelectModels): void {
    if (row.id) {
      this.sweetAlert.showConfirm('¿Está seguro?', 'Esta acción no se puede deshacer').then(result => {
        if (result.isConfirmed) {
          this.store.delete(row.id).subscribe({
            next: () => {
              this.sweetAlert.showNotification('Éxito', 'Ciudad eliminada exitosamente', 'success');
            }
          });
        }
      });
    }
  }


  // ----- Toggle estado (activo/inactivo) -----
  onToggleActive(row: FinanceSelectModels, e: { checked: boolean }) {
    const previous = row.active;
    row.active = e.checked;
    this.store.changeActiveStatus(row.id, e.checked).subscribe({
      next: (updated) => {
        // sincronizar con lo que devuelve el backend
        row.active = updated.active ?? row.active;
        this.sweetAlert.showNotification(
          'Éxito',
          `Ciudad ${row.active ? 'activado' : 'desactivado'} correctamente.`,
          'success'
        );
      },
      error: (err) => {
        // revertir si falla
        row.active = previous;
        this.sweetAlert.showNotification(
          'Error',
          err?.error?.detail || 'No se pudo cambiar el estado.',
          'error'
        );
      }
    });
  }
}
