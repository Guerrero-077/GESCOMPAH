import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { Observable } from 'rxjs';
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { DepartmentStore } from '../../services/department/department.store';
import { DepartmentSelectModel } from '../../models/department.models';
import { ToggleButtonComponent } from "../../../../shared/components/toggle-button-component/toggle-button-component.component";
import { HasRoleAndPermissionDirective } from '../../../../core/Directives/HasRoleAndPermission.directive';


@Component({
  selector: 'app-department',
  standalone: true,
  imports: [
    CommonModule,
    GenericTableComponent,
    MatButtonModule,
    MatIconModule,
    ToggleButtonComponent,
    HasRoleAndPermissionDirective
  ],
  templateUrl: './department.component.html',
  styleUrls: ['./department.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DepartmentComponent implements OnInit {
  departments$: Observable<DepartmentSelectModel[]>;

  @ViewChild('estadoTemplate', { static: true }) estadoTemplate!: TemplateRef<any>;

  columns: TableColumn<DepartmentSelectModel>[] = [
    { key: 'id', header: 'ID' },
    { key: 'name', header: 'Nombre' }
  ];

  constructor(
    private store: DepartmentStore,
    private dialog: MatDialog,
    private sweetAlert: SweetAlertService
  ) {
    this.departments$ = this.store.departments$;
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
        formType: 'Department'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.store.create(result).subscribe({
          next: () => {
            this.sweetAlert.showNotification('Éxito', 'Departamento creado exitosamente', 'success');
          }
        });
      }
    });
  }

  openEditDialog(row: DepartmentSelectModel): void {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '400px',
      data: {
        entity: row,
        formType: 'Department'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.store.update(result).subscribe({
          next: () => {
            this.sweetAlert.showNotification('Éxito', 'Departamento actualizado exitosamente', 'success');
          }
        });
      }
    });
  }

  handleDelete(row: DepartmentSelectModel): void {
    if (row.id) {
      this.sweetAlert.showConfirm('¿Está seguro?', 'Esta acción no se puede deshacer').then(result => {
        if (result.isConfirmed) {
          this.store.delete(row.id).subscribe({
            next: () => {
              this.sweetAlert.showNotification('Éxito', 'Departamento eliminado exitosamente', 'success');
            }
          });
        }
      });
    }
  }

  // ----- Toggle estado (activo/inactivo) -----
  onToggleActive(row: DepartmentSelectModel, e: { checked: boolean }) {
    const previous = row.active;
    row.active = e.checked;
    this.store.changeActiveStatus(row.id, e.checked).subscribe({
      next: (updated) => {
        // sincronizar con lo que devuelve el backend
        row.active = updated.active ?? row.active;
        this.sweetAlert.showNotification(
          'Éxito',
          `Departamento ${row.active ? 'activado' : 'desactivado'} correctamente.`,
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
