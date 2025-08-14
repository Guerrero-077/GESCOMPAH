import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { CityStore } from '../../services/city/city.store';
import { Observable } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service'
import { CitySelectModel } from '../../models/city.models';


@Component({
  selector: 'app-city',
  standalone: true,
  imports: [
    CommonModule,
    GenericTableComponent,
    MatButtonModule,
    MatIconModule,
  ],
  templateUrl: './city.component.html',
  styleUrls: ['./city.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CityComponent implements OnInit {
  cities$: Observable<CitySelectModel[]>;
  columns: TableColumn<CitySelectModel>[] = [
    { key: 'id', header: 'ID' },
    { key: 'name', header: 'Nombre' },
    { key: 'departmentName', header: 'Departamento' },
    { key: 'active', header: 'Activo', type: 'boolean' }
  ];

  constructor(
    private store: CityStore,
    private dialog: MatDialog,
    private sweetAlert: SweetAlertService
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
            this.sweetAlert.showNotification('Éxito', 'Ciudad creada exitosamente', 'success');
          }
        });
      }
    });
  }

  openEditDialog(row: CitySelectModel): void {
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
            this.sweetAlert.showNotification('Éxito', 'Ciudad actualizada exitosamente', 'success');
          }
        });
      }
    });
  }

  handleDelete(row: CitySelectModel): void {
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
}
