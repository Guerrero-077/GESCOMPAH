import { Component, OnDestroy, OnInit, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { take, switchMap, tap, finalize } from 'rxjs/operators';
import { of } from 'rxjs';

import { ContractCard } from '../../models/contract.models';
import { ContractStore } from '../../services/contract/contract.store';
import { ContractService } from '../../services/contract/contract.service';
// import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { PageHeaderService } from '../../../../shared/Services/PageHeader/page-header.service';

import { ToggleButtonComponent } from '../../../../shared/components/toggle-button-component/toggle-button-component.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { HasRoleAndPermissionDirective } from '../../../../core/security/directives/HasRoleAndPermission.directive';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

import { ContractsRealtimeService } from '../../../../core/realtime/contracts-realtime.service';
import { StandardButtonComponent } from '../../../../shared/components/standard-button/standard-button.component';
import { MoneyPipe } from '../../../../shared/pipes/money.pipe';

@Component({
  selector: 'app-contracts-list',
  standalone: true,
  imports: [
    CommonModule,
    // UI
    MatDialogModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatButtonModule,
    MatTooltipModule,
    MatFormFieldModule,
    MatInputModule,
    // Custom
    ToggleButtonComponent,
    HasRoleAndPermissionDirective,
    StandardButtonComponent,
    MoneyPipe,
  ],
  templateUrl: './contracts-list.component.html',
  styleUrls: ['./contracts-list.component.css'],
})
export class ContractsListComponent implements OnInit, OnDestroy {
  private readonly store = inject(ContractStore);
  private readonly svc = inject(ContractService);
  private readonly dialog = inject(MatDialog);
  // private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly toast = inject(SweetAlertService);
  private readonly pageHeader = inject(PageHeaderService);
  private readonly realtime = inject(ContractsRealtimeService);

  readonly rows = this.store.items;       // signal<readonly ContractCard[]>
  readonly loading = this.store.loading;  // signal<boolean>
  readonly error = this.store.error;      // signal<string|null>

  // --- Filtro y derivadas ---
  readonly filterKey = signal<string>('');

  // ✅ Nota: devolvemos "readonly ContractCard[]" para no chocar con store.items
  readonly filtered = computed<readonly ContractCard[]>(() => {
    const list = this.rows() ?? [];
    const q = this.filterKey().trim().toLowerCase();
    if (!q) return list; // ya es readonly
    // Array.filter<T> devuelve T[], que es asignable a readonly T[]
    return list.filter(it => (
      it.personFullName?.toLowerCase().includes(q) ||
      it.personDocument?.toLowerCase().includes(q) ||
      it.personPhone?.toLowerCase().includes(q) ||
      (it.personEmail || '').toLowerCase().includes(q)
    ));
  });

  readonly totalCount = computed(() => (this.rows()?.length ?? 0));
  readonly activeCount = computed(() => (this.rows()?.filter(x => x.active).length ?? 0));
  readonly inactiveCount = computed(() => (this.rows()?.filter(x => !x.active).length ?? 0));

  isDownloadingPdf = false;

  async ngOnInit(): Promise<void> {
    this.pageHeader.setPageHeader('Contratos', 'Gestión de Contratos');
    this.realtime.connect();
    await this.store.loadAll({ force: true });
  }

  ngOnDestroy(): void {
    this.realtime.disconnect();
  }

  onFilterChange(v: string): void {
    this.filterKey.set(v || '');
  }

  onCreate(): void {
    import('../../components/form-contract/form-contract.component').then(m => {
      const ref = this.dialog.open(m.FormContractComponent, {
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
    });
  }

  onView(row: ContractCard): void {
    import('../../components/contract-detail-dialog/contract-detail-dialog.component').then(m => {
      this.dialog.open(m.ContractDetailDialogComponent, {
        width: '900px',
        maxWidth: '95vw',
        data: { id: row.id },
        autoFocus: false,
        disableClose: false,
      });
    });
  }

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
        this.toast.showApiError(err, 'No se pudo descargar el contrato.');
      },
    });
  }

  async onToggleActive(row: ContractCard, e: { checked: boolean } | boolean): Promise<void> {
    const next = typeof e === 'boolean' ? e : !!e?.checked;
    try {
      await this.store.changeActiveStatusRemote(row.id, next);
      this.toast.showNotification('Éxito', `Contrato ${next ? 'activado' : 'desactivado'} correctamente.`, 'success');
    } catch (err: any) {
      this.toast.showApiError(err, 'No se pudo cambiar el estado.');
    }
  }

  async onDelete(row: ContractCard): Promise<void> {
    const result = await this.toast.showConfirm('Eliminar contrato', `¿Deseas eliminar el contrato de "${row.personFullName}"?`, 'Eliminar', 'Cancelar');
    if (!result.isConfirmed) return;

    try {
      await this.store.delete(row.id);
      this.toast.showNotification('Eliminación Exitosa', 'Contrato eliminado correctamente.', 'success');
    } catch (err) {
      this.toast.showApiError(err, 'No se pudo eliminar el contrato.');
    }
  }

  trackById = (_: number, item: ContractCard) => item.id;

  // Notifica error de carga inicial/stream mediante toast en lugar de un div en plantilla
  private readonly errorToast = effect(() => {
    const err = this.error?.();
    if (err) {
      this.toast.showApiError(err, 'No se pudieron cargar los contratos.');
    }
  });
}
