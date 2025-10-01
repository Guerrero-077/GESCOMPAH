import { Component, TemplateRef, ViewChild } from '@angular/core';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { ToggleButtonComponent } from "../../../../shared/components/toggle-button-component/toggle-button-component.component";
import { MatDialog } from '@angular/material/dialog';
import { Observable, take } from 'rxjs';
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { FinanceSelectModels } from '../../models/finance.models';
import { CommonModule } from '@angular/common';
import { FinanceStore } from '../../services/finance/finance.store';
import { IsActive } from '../../../../core/models/IsAcitve.models';

@Component({
  selector: 'app-finance',
  imports: [GenericTableComponent, ToggleButtonComponent, CommonModule],
  templateUrl: './finance.component.html',
  styleUrls: ['./finance.component.css']
})
export class FinanceComponent {
  finances$: Observable<FinanceSelectModels[]>;

  @ViewChild('estadoTemplate', { static: true }) estadoTemplate!: TemplateRef<any>;

  columns: TableColumn<FinanceSelectModels>[] = [
    { key: 'id', header: 'ID' },
    { key: 'key', header: 'Nombre' },
    { key: 'value', header: 'valor' },
    { key: 'effectiveFrom', header: 'Vigente desde', format: (v) => this.formatDateOnly(v) },
    { key: 'effectiveTo', header: 'Vigente hasta', format: (v) => this.formatDateOnly(v) },
  ];

  constructor(
    private store: FinanceStore,
    private dialog: MatDialog,
    private sweetAlert: SweetAlertService
  ) {
    this.finances$ = this.store.finances$;
  }

  // Formatea a YYYY-MM-DD (sin hora)
  private formatDateOnly(v: any): string {
    if (!v) return '';
    try {
      const d = new Date(v);
      if (isNaN(d.getTime())) return String(v);
      return d.toISOString().split('T')[0];
    } catch { return String(v); }
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


  // Toggle activo/inactivo (UI optimista + rollback)
  onToggleActive(row: IsActive, e: boolean | { checked: boolean }) {
    const checked = typeof e === 'boolean' ? e : !!e?.checked;
    const prev = row.active;

    // UI optimista
    row.active = checked;

    this.store.changeActiveStatus(row.id, checked).pipe(take(1)).subscribe({
      next: updated => {
        // Si la API devuelve 204, mantenemos el valor; si devuelve DTO, sincronizamos
        row.active = updated?.active ?? checked;
        this.sweetAlert.showNotification('Éxito',
          `Departamento ${row.active ? 'activado' : 'desactivado'} correctamente.`,
          'success');
      },
      error: err => {
        // Rollback
        row.active = prev;
        this.sweetAlert.showApiError(err, 'No se pudo cambiar el estado.');
      }
    });
  }


}
