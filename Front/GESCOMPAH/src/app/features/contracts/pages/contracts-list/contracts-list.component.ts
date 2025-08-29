// src/app/features/contracts/components/contracts-list/contracts-list.component.ts
import { Component, inject, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialog, MatDialogModule } from '@angular/material/dialog'; // ⬅️ AÑADIDO
import { EMPTY } from 'rxjs';
import { catchError, take, finalize } from 'rxjs/operators';

// Models
import { ContractSelectModel } from '../../models/contract.models';
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
    MatDialogModule, // ⬅️ AÑADIDO
  ],
  templateUrl: './contracts-list.component.html',
  styleUrls: ['./contracts-list.component.css']
})
export class ContractsListComponent implements OnInit {

  private readonly contractService = inject(ContractService);
  private readonly dialog = inject(MatDialog);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly sweetAlertService = inject(SweetAlertService);

  contracts$ = this.contractService.contracts;

  columns: TableColumn<ContractSelectModel>[] = [];

  /** Flag de loading mientras se genera/descarga el PDF */
  isDownloadingPdf = false;

  @ViewChild('estadoTemplate', { static: true }) estadoTemplate!: TemplateRef<any>;

  ngOnInit(): void {
    this.loadContracts();
    this.setupColumns();
  }

  loadContracts(): void {
    this.contractService.getAllContracts({ force: true }).pipe(
      take(1),
      catchError(err => {
        console.error('Error loading contracts:', err);
        this.sweetAlertService.showNotification('Error', 'No se pudieron cargar los contratos.', 'error');
        return EMPTY;
      })
    ).subscribe();
  }

  setupColumns(): void {
    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
      { key: 'fullName', header: 'Arrendatario' },
      { key: 'document', header: 'Documento' },
      { key: 'startDate', header: 'Fecha Inicio' },
      { key: 'endDate', header: 'Fecha Fin' },
      {
        key: 'active',
        header: 'Estado',
        type: 'custom',
        template: this.estadoTemplate
      }
    ];
  }

  /** ✅ Crea: abre diálogo, crea contrato y recarga */
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
        this.loadContracts(); // recargar lista
      }
    });
  }

  onEdit(row: ContractSelectModel) {
    // (tu lógica actual)
  }

  async onDelete(row: ContractSelectModel) {
    // (tu lógica actual)
  }

  /** Descargar PDF mostrando overlay con spinner + animación */
  onView(row: ContractSelectModel) {
    if (this.isDownloadingPdf) {
      this.sweetAlertService.showNotification('Información', 'Ya hay una descarga en curso.', 'info');
      return;
    }

    this.isDownloadingPdf = true;

    this.contractService.downloadContractPdf(row.id).pipe(
      take(1),
      finalize(() => this.isDownloadingPdf = false)
    ).subscribe({
      next: blob => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `Contrato_${row.fullName}.pdf`;
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
        document.body.removeChild(a);
        this.sweetAlertService.showNotification('Éxito', 'La descarga del contrato ha comenzado.', 'success');
      },
      error: err => {
        console.error('Error downloading PDF:', err);
        this.sweetAlertService.showNotification('Error', 'No se pudo descargar el contrato.', 'error');
      }
    });
  }

  // ----- Toggle estado (activo/inactivo) -----
  onToggleActive(row: ContractSelectModel, e: { checked: boolean }) {
    const previous = row.active;
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
        row.active = previous;
        this.sweetAlertService.showNotification(
          'Error',
          err?.error?.detail || 'No se pudo cambiar el estado.',
          'error'
        );
      }
    });
  }

}
