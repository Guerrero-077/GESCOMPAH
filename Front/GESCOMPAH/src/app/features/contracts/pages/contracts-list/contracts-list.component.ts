import { Component, OnInit, TemplateRef, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { take, switchMap, tap, finalize } from 'rxjs/operators';
import { of } from 'rxjs';

import { ContractCard } from '../../models/contract.models';
import { TableColumn } from '../../../../shared/models/TableColumn.models';

import { ContractStore } from '../../services/contract/contract.store';
import { ContractService } from '../../services/contract/contract.service';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { PageHeaderService } from '../../../../shared/Services/PageHeader/page-header.service';

import { GenericTableComponent } from '../../../../shared/components/generic-table/generic-table.component';
import { ToggleButtonComponent } from '../../../../shared/components/toggle-button-component/toggle-button-component.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { FormContractComponent } from '../../components/form-contract/form-contract.component';
import { HasRoleAndPermissionDirective } from '../../../../core/Directives/HasRoleAndPermission.directive';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-contracts-list',
  standalone: true,
  imports: [
    CommonModule,
    GenericTableComponent,
    ToggleButtonComponent,
    MatProgressSpinnerModule,
    MatDialogModule,
    HasRoleAndPermissionDirective,

    MatIconModule,
    MatButtonModule,
    MatTooltipModule,
  ],
  templateUrl: './contracts-list.component.html',
  styleUrls: ['./contracts-list.component.css'],
})
export class ContractsListComponent implements OnInit {
  private readonly store = inject(ContractStore);
  private readonly svc = inject(ContractService);
  private readonly dialog = inject(MatDialog);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly toast = inject(SweetAlertService);
  private readonly pageHeader = inject(PageHeaderService);

  readonly rows = this.store.items;
  readonly loading = this.store.loading;
  readonly error = this.store.error;

  columns: TableColumn<ContractCard>[] = [];
  isDownloadingPdf = false;

  // IMPORTANTE: al estar FUERA de *ngIf, podemos usar static:true
  @ViewChild('estadoTemplate', { static: true }) estadoTemplate!: TemplateRef<any>;

  async ngOnInit(): Promise<void> {
    this.pageHeader.setPageHeader('Contratos', 'Gestión de Contratos');
    this.setupColumns();
    await this.store.loadAll({ force: true });
  }

  private setupColumns(): void {
    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
      { key: 'personFullName', header: 'Arrendatario' },
      { key: 'personDocument', header: 'Documento' },
      { key: 'personPhone', header: 'Teléfono' },
      { key: 'personEmail', header: 'Email' },
      { key: 'startDate', header: 'Inicio' },
      { key: 'endDate', header: 'Fin' },
      { key: 'totalBase', header: 'Total Base' },
      { key: 'totalUvt', header: 'Total UVT' },
      { key: 'active', header: 'Estado', type: 'custom', template: this.estadoTemplate },
    ];
  }

  onCreateNew(): void {
    const ref = this.dialog.open(FormContractComponent, {
      width: '800px',
      disableClose: true,
      autoFocus: true,
      data: null,
    });

    ref.afterClosed().pipe(take(1)).subscribe(async (created: boolean) => {
      if (!created) return;
      this.toast.showNotification('Éxito', 'Contrato creado correctamente.', 'success');
      await this.store.loadAll({ force: true });
    });
  }

  onView(row: ContractCard): void {}


  onDownload(row: ContractCard): void {
    if (this.isDownloadingPdf) {
      this.toast.showNotification('Información', 'Ya hay una descarga en curso.', 'info');
      return;
    }
    this.isDownloadingPdf = true;

    const fileName$ = of(`Contrato_${row.personFullName || row.id}.pdf`);
    this.svc.downloadContractPdf(row.id).pipe(
      take(1),
      switchMap(blob =>
        fileName$.pipe(
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
        )
      ),
      finalize(() => (this.isDownloadingPdf = false))
    ).subscribe({
      next: () => this.toast.showNotification('Éxito', 'La descarga del contrato ha comenzado.', 'success'),
      error: (err) => {
        console.error('Error downloading PDF:', err);
        this.toast.showNotification('Error', 'No se pudo descargar el contrato.', 'error');
      },
    });
  }
  async onToggleActive(row: ContractCard, e: { checked: boolean } | boolean): Promise<void> {
    const next = typeof e === 'boolean' ? e : !!e?.checked;
    try {
      await this.store.changeActiveStatusRemote(row.id, next);
      this.toast.showNotification('Éxito', `Contrato ${next ? 'activado' : 'desactivado'} correctamente.`, 'success');
    } catch (err: any) {
      this.toast.showNotification('Error', err?.message || 'No se pudo cambiar el estado.', 'error');
    }
  }


  async onDelete(row: ContractCard): Promise<void> {
    const ok = await this.confirmDialog.confirm({
      title: 'Eliminar contrato',
      text: `¿Deseas eliminar el contrato de "${row.personFullName}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar',
    });
    if (!ok) return;

    try {
      await this.store.delete(row.id);
      this.toast.showNotification('Eliminación Exitosa', 'Contrato eliminado correctamente.', 'success');
    } catch {
      this.toast.showNotification('Error', 'No se pudo eliminar el contrato.', 'error');
    }
  }

  trackById = (_: number, item: ContractCard) => item.id;
}
