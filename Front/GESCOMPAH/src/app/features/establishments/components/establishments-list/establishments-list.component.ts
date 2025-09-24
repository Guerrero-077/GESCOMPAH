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
import { EstablishmentService } from '../../services/establishment/establishment.service';
import { EstablishmentStore } from '../../services/establishment/establishment.store';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { SharedEventsServiceService } from '../../services/shared/shared-events-service.service';
import { HasRoleAndPermissionDirective } from '../../../../core/security/directives/HasRoleAndPermission.directive';
import { StandardButtonComponent } from '../../../../shared/components/standard-button/standard-button.component';

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
    HasRoleAndPermissionDirective,
    StandardButtonComponent
  ],
  templateUrl: './establishments-list.component.html',
  styleUrls: ['./establishments-list.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EstablishmentsListComponent implements OnInit {
  private readonly dialog = inject(MatDialog);
  private readonly store = inject(EstablishmentStore);
  private readonly estSvc = inject(EstablishmentService);
  private readonly sweetAlert = inject(SweetAlertService);
  private readonly sharedEvents = inject(SharedEventsServiceService);
  private readonly destroyRef = inject(DestroyRef);

  // Señales listas para plantilla/ts
  // Cards livianos desde backend
  readonly cards = this.store.cards;
  readonly loadingCards = this.store.cardsLoading;
  readonly errorCards = this.store.cardsError;

  async ngOnInit(): Promise<void> {
    // Carga inicial de cards livianos
    await this.store.loadCardsAll();

    // Si otro módulo notifica cambios, refresca
    this.sharedEvents.plazaStateChanged$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => { void this.store.loadCardsAll(); });
  }

  openCreateDialog(): void {
    import('../form-establishment/form-establishment.component').then(m => {
      this.dialog.closeAll(); // cierra otros diálogos abiertos
      const ref = this.dialog.open(m.FormEstablishmentComponent, {
        id: 'createDialog',
        width: '600px',
        data: null
      });
      ref.afterClosed().subscribe(async ok => {
        if (ok) await this.store.loadCardsAll();
      });
    });
  }


  async handleDelete(id: number): Promise<void> {
    const result = await this.sweetAlert.showConfirm('¿Está seguro?', 'Esta acción no se puede deshacer');
    if (!result.isConfirmed) return;

    try {
      await this.store.delete(id);
      this.sweetAlert.showNotification('Éxito', 'Local eliminado exitosamente', 'success');
      await this.store.loadCardsAll();
    } catch (err: any) {
      this.sweetAlert.showNotification('Error', err?.message || 'No se pudo eliminar', 'error');
    }
  }

  onCardUpdated(): void { void this.store.loadCardsAll(); }

  onView(id: number): void {
    if (this.dialog.getDialogById('viewDialog')) {
      return;
    }

    this.estSvc.getById(id).subscribe({
      next: row => {
        import('../establishment-detail-dialog/establishment-detail-dialog.component').then(m => {
          this.dialog.open(m.EstablishmentDetailDialogComponent, {
            id: 'viewDialog',
            width: '900px',
            data: row
          });
        });
      },
      error: () =>
        this.sweetAlert.showNotification('Error', 'Establecimiento no encontrado', 'error')
    });
  }

  openEditDialogById(id: number): void {
    this.estSvc.getById(id).subscribe({
      next: row => {
        import('../form-establishment/form-establishment.component').then(m => {
          this.dialog.closeAll(); // fuerza que solo quede este abierto
          const ref = this.dialog.open(m.FormEstablishmentComponent, {
            id: 'editDialog',
            width: '600px',
            data: row
          });
          ref.afterClosed().subscribe(async ok => {
            if (ok) await this.store.loadCardsAll();
          });
        });
      },
      error: () =>
        this.sweetAlert.showNotification('Error', 'Establecimiento no encontrado', 'error')
    });
  }

}

