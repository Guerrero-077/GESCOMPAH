import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, inject, DestroyRef } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatStepperModule } from '@angular/material/stepper';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { CardComponent } from '../../../../shared/components/card/card.component';
import { EstablishmentSelect } from '../../models/establishment.models';
import { EstablishmentStore } from '../../services/establishment/establishment.store';
import { FormEstablishmentComponent } from '../form-establishment/form-establishment.component';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { EstablishmentDetailDialogComponent } from '../establishment-detail-dialog/establishment-detail-dialog.component';
import { SharedEventsServiceService } from '../../services/shared/shared-events-service.service';
import { HasRoleAndPermissionDirective } from '../../../../core/Directives/HasRoleAndPermission.directive';

@Component({
  selector: 'app-establishments-list',
  imports: [
    CommonModule,
    CardComponent,
    ReactiveFormsModule,
    MatDialogModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatStepperModule,
    MatIconModule,
    MatProgressSpinnerModule,
    HasRoleAndPermissionDirective
  ],
  templateUrl: './establishments-list.component.html',
  styleUrls: ['./establishments-list.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EstablishmentsListComponent implements OnInit {
  private readonly dialog = inject(MatDialog);
  private readonly store = inject(EstablishmentStore);
  private readonly sweetAlert = inject(SweetAlertService);
  private readonly sharedEvents = inject(SharedEventsServiceService);
  private readonly destroyRef = inject(DestroyRef);

  // Señales listas para plantilla/ts
  readonly establishments = this.store.view;     // lista según activeOnlyView
  readonly loading = this.store.loading;
  readonly error = this.store.error;

  async ngOnInit(): Promise<void> {
    // Carga inicial sin subscribes colgantes
    await this.store.loadAll();

    // Si otro módulo notifica cambios, refresca
    this.sharedEvents.plazaStateChanged$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => { void this.store.loadAll(); });
  }

  openCreateDialog(): void {
    const ref = this.dialog.open(FormEstablishmentComponent, {
      width: '600px',
      data: null
    });
    ref.afterClosed().subscribe(async ok => {
      if (ok) await this.store.loadAll();
    });
  }

  openEditDialog(est: EstablishmentSelect): void {
    const ref = this.dialog.open(FormEstablishmentComponent, {
      width: '600px',
      data: est
    });
    ref.afterClosed().subscribe(async ok => {
      if (ok) await this.store.loadAll();
    });
  }

  async handleDelete(id: number): Promise<void> {
    const result = await this.sweetAlert.showConfirm('¿Está seguro?', 'Esta acción no se puede deshacer');
    if (!result.isConfirmed) return;

    try {
      await this.store.delete(id);
      this.sweetAlert.showNotification('Éxito', 'Local eliminado exitosamente', 'success');
    } catch (err: any) {
      this.sweetAlert.showNotification('Error', err?.message || 'No se pudo eliminar', 'error');
    }
  }

  onCardUpdated(): void {
    void this.store.loadAll();
  }

  onView(id: number): void {
    const row = this.establishments().find(e => e.id === id);
    if (row) {
      this.dialog.open(EstablishmentDetailDialogComponent, {
        width: '900px',
        data: row
      });
    } else {
      this.sweetAlert.showNotification('Error', 'Establecimiento no encontrado', 'error');
    }
  }
}
