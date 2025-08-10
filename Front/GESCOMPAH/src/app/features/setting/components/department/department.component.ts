import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { DepartmentModel } from '../../models/department.models';
import { DepartmentStore } from '../../services/department/department.store';
import { Observable } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';

@Component({
  selector: 'app-department',
  standalone: true,
  imports: [
    CommonModule,
    GenericTableComponent,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule
  ],
  templateUrl: './department.component.html',
  styleUrl: './department.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DepartmentComponent implements OnInit {
  departments$: Observable<DepartmentModel[]>;
  columns: TableColumn<DepartmentModel>[] = [
    { key: 'id', header: 'ID' },
    { key: 'name', header: 'Nombre' },
    { key: 'active', header: 'Activo', type: 'boolean' }
  ];

  constructor(
    private store: DepartmentStore,
    private dialog: MatDialog,
    private snackbar: MatSnackBar
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
            this.snackbar.open('Departamento creado exitosamente', 'Cerrar', { duration: 2000 });
          }
        });
      }
    });
  }

  openEditDialog(row: DepartmentModel): void {
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
            this.snackbar.open('Departamento actualizado exitosamente', 'Cerrar', { duration: 2000 });
          }
        });
      }
    });
  }

  handleDelete(row: DepartmentModel): void {
    if (row.id) {
      this.store.delete(row.id).subscribe({
        next: () => {
          this.snackbar.open('Departamento eliminado exitosamente', 'Cerrar', { duration: 2000 });
        }
      });
    }
  }
}
