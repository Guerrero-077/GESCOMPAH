import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { CityModel } from '../../models/city.models';
import { CityStore } from '../../services/city/city.store';
import { Observable } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';

@Component({
  selector: 'app-city',
  standalone: true,
  imports: [
    CommonModule,
    GenericTableComponent,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule
  ],
  templateUrl: './city.component.html',
  styleUrl: './city.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CityComponent implements OnInit {
  cities$: Observable<CityModel[]>;
  columns: TableColumn<CityModel>[] = [
    { key: 'id', header: 'ID' },
    { key: 'name', header: 'Nombre' },
    { key: 'departmentName', header: 'Departamento' },
    { key: 'active', header: 'Activo', type: 'boolean' }
  ];

  constructor(
    private store: CityStore,
    private dialog: MatDialog,
    private snackbar: MatSnackBar
  ) {
    this.cities$ = this.store.cities$;
  }

  ngOnInit(): void {
    // Columns are initialized directly now
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '400px',
      data: {
        entity: {},
        formType: 'City'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.store.create(result).subscribe({
          next: () => {
            this.snackbar.open('Ciudad creada exitosamente', 'Cerrar', { duration: 2000 });
          }
        });
      }
    });
  }

  openEditDialog(row: CityModel): void {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '400px',
      data: {
        entity: row,
        formType: 'City'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.store.update(result).subscribe({
          next: () => {
            this.snackbar.open('Ciudad actualizada exitosamente', 'Cerrar', { duration: 2000 });
          }
        });
      }
    });
  }

  handleDelete(row: CityModel): void {
    if (row.id) {
      this.store.delete(row.id).subscribe({
        next: () => {
          this.snackbar.open('Ciudad eliminada exitosamente', 'Cerrar', { duration: 2000 });
        }
      });
    }
  }
}