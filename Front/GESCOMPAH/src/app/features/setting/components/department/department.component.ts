import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
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


@Component({
  selector: 'app-department',
  standalone: true,
  imports: [
    CommonModule,
    GenericTableComponent,
    MatButtonModule,
    MatIconModule,
  ],
  templateUrl: './department.component.html',
  styleUrls: ['./department.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DepartmentComponent implements OnInit {
  departments$: Observable<DepartmentSelectModel[]>;
  columns: TableColumn<DepartmentSelectModel>[] = [
    { key: 'id', header: 'ID' },
    { key: 'name', header: 'Nombre' },
    { key: 'active', header: 'Activo', type: 'boolean' }
  ];

  constructor(
    private store: DepartmentStore,
    private dialog: MatDialog,
    private sweetAlert: SweetAlertService
  ) {
    this.departments$ = this.store.departments$;
  }

  ngOnInit(): void {
    // Columns are initialized directly now
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
}
