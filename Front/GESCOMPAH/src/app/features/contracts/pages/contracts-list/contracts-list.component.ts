// src/app/features/contracts/components/contracts-list/contracts-list.component.ts
import { Component, inject, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { EMPTY, of } from 'rxjs';
import { catchError, take, finalize, switchMap, tap } from 'rxjs/operators';

// Models
import { ContractCard, ContractSelectModel } from '../../models/contract.models';
import { TableColumn } from '../../../../shared/models/TableColumn.models';

// Services
import { ContractService } from '../../services/contract/contract.service';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';

// Components
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { ToggleButtonComponent } from "../../../../shared/components/toggle-button-component/toggle-button-component.component";

// Angular Material
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { FormContractComponent } from '../../components/form-contract/form-contract.component';

@Component({
  selector: 'app-contracts-list',
  standalone: true,
  imports: [
    CommonModule,
    GenericTableComponent,
    ToggleButtonComponent,
    MatProgressSpinnerModule,
    MatDialogModule,
  ],
  templateUrl: './contracts-list.component.html',
  styleUrls: ['./contracts-list.component.css']
})
export class ContractsListComponent implements OnInit {

  private readonly contractService = inject(ContractService);
  private readonly dialog = inject(MatDialog);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly sweetAlertService = inject(SweetAlertService);

  // Única fuente de datos para la tabla
  rows$ = this.contractService.rows;

  // Columnas para ContractCard
  columns: TableColumn<ContractCard>[] = [];

  /** Flag de loading mientras se genera/descarga el PDF */
  isDownloadingPdf = false;

  @ViewChild('estadoTemplate', { static: true }) estadoTemplate!: TemplateRef<any>;

  ngOnInit(): void {
    this.setupColumns();
    this.contractService.getList({ force: true }).pipe(
      take(1),
      catchError(err => {
        console.error('Error loading contracts:', err);
        this.sweetAlertService.showNotification('Error', 'No se pudieron cargar los contratos.', 'error');
        return EMPTY;
      })
    ).subscribe();
  }

  private setupColumns(): void {
    this.columns = [
      { key: 'index',            header: 'Nº', type: 'index' },
      { key: 'personFullName',   header: 'Arrendatario' },
      { key: 'personDocument',   header: 'Documento' },
      { key: 'personPhone',      header: 'Teléfono' },
      { key: 'personEmail',      header: 'Email' },
      { key: 'startDate',        header: 'Inicio' },
      { key: 'endDate',          header: 'Fin' },
      { key: 'totalBase',        header: 'Total Base' },
      { key: 'totalUvt',         header: 'Total UVT' },
      { key: 'active',           header: 'Estado', type: 'custom', template: this.estadoTemplate }
    ];
  }

  /** Crear */
  onCreateNew() {
    const ref = this.dialog.open(FormContractComponent, {
      width: '800px',
      disableClose: true,
      autoFocus: true,
      data: null
    });

    ref.afterClosed().pipe(take(1)).subscribe((created: boolean) => {
      if (created) {
        this.sweetAlertService.showNotification('Éxito', 'Contrato creado correctamente.', 'success');
        this.contractService.getList({ force: true }).pipe(take(1)).subscribe();
      }
    });
  }

  /** Descargar PDF */
  onView(row: ContractCard) {
    if (this.isDownloadingPdf) {
      this.sweetAlertService.showNotification('Información', 'Ya hay una descarga en curso.', 'info');
      return;
    }
    this.isDownloadingPdf = true;

    const fileName$ = of(`Contrato_${row.personFullName || row.id}.pdf`);

    this.contractService.downloadContractPdf(row.id).pipe(
      take(1),
      switchMap(blob => fileName$.pipe(
        take(1),
        tap((fileName: string) => {
          const url = window.URL.createObjectURL(blob);
          const a = document.createElement('a');
          a.href = url;
          a.download = fileName;
          document.body.appendChild(a);
          a.click();
          window.URL.revokeObjectURL(url);
          document.body.removeChild(a);
        })
      )),
      finalize(() => this.isDownloadingPdf = false)
    ).subscribe({
      next: () => this.sweetAlertService.showNotification('Éxito', 'La descarga del contrato ha comenzado.', 'success'),
      error: err => {
        console.error('Error downloading PDF:', err);
        this.sweetAlertService.showNotification('Error', 'No se pudo descargar el contrato.', 'error');
      }
    });
  }

  // Toggle activo/inactivo
  onToggleActive(row: ContractCard, e: { checked: boolean }) {
    const nextValue = e.checked;
    this.contractService.updateContractActive(row.id, nextValue).subscribe({
      next: (updated) => {
        this.sweetAlertService.showNotification(
          'Éxito',
          `Contrato ${updated.active ? 'activado' : 'desactivado'} correctamente.`,
          'success'
        );
      },
      error: (err) => {
        this.sweetAlertService.showNotification(
          'Error',
          err?.error?.detail || 'No se pudo cambiar el estado.',
          'error'
        );
      }
    });
  }
}
