import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import {
  Component,
  DestroyRef,
  inject,
  OnInit,
  signal,
  computed
} from '@angular/core';
import { RouterLink } from '@angular/router';
import { CardComponent } from '../../../../shared/components/card/card.component';
import { EstablishmentStore } from '../../../establishments/services/establishment/establishment.store';
import { MatDialog } from '@angular/material/dialog';
import { EstablishmentService } from '../../../establishments/services/establishment/establishment.service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';

import { AppointmentService } from '../../../appointment/services/appointment/appointment.service';

import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatInputModule } from "@angular/material/input";
import { MatSelectModule } from "@angular/material/select";
import { NonNullableFormBuilder, Validators, AbstractControl } from '@angular/forms';
import { AppValidators } from '../../../../shared/utils/AppValidators';
import { SquareService } from '../../../establishments/services/square/square.service';
import { SquareSelectModel } from '../../../establishments/models/squares.models';
import { catchError, finalize, of, Subject, takeUntil } from 'rxjs';
import { EstablishmentCard } from '../../../establishments/models/establishment.models';


@Component({
  selector: 'app-establishment-list',
  imports: [CommonModule, CardComponent, RouterLink, MatToolbarModule, MatButtonModule, MatIconModule, MatInputModule, MatSelectModule],
  templateUrl: './establishment-list.component.html',
  styleUrl: './establishment-list.component.css'
})
export class EstablishmentListComponent implements OnInit {

  private readonly fb = inject(NonNullableFormBuilder);
  private readonly store = inject(EstablishmentStore);
  private readonly estSvc = inject(EstablishmentService);
  private readonly squareSvc = inject(SquareService);
  private readonly sweetAlert = inject(SweetAlertService);
  private readonly dialog = inject(MatDialog);
  private readonly destroyRef = inject(DestroyRef);
  private readonly router = inject(Router);

  private readonly destroy$ = new Subject<void>();
  readonly items = this.store.items;
  readonly rows = this.store.items;

  plazas: SquareSelectModel[] = [];
  establishments: EstablishmentCard[] = [];
  selectedPlazaId: number | null = null;

  readonly filteredEstablishments = signal<EstablishmentCard[]>([]);

  loadingEstablishments = false;

  async ngOnInit(): Promise<void> {
    await this.store.loadAll({ activeOnly: true });
    this.loadPlazas();
  }

  onView(id: number): void {
    import('../../../establishments/services/establishment/establishment.service').then(({ EstablishmentService }) => {
      const service = (this as any).store['svc'] as EstablishmentService;
      service.getById(id).subscribe({
        next: row => {
          import('../../../establishments/components/establishment-detail-dialog/establishment-detail-dialog.component').then(m => {
            this.dialog.open(m.EstablishmentDetailDialogComponent, {
              width: '900px',
              data: row
            });
          });
        },
        error: () => this.sweetAlert.showNotification('Error', 'Establecimiento no encontrado', 'error')
      });
    });
  }

  onCreateAppointment(id: number): void {
    import('../../../appointment/services/appointment/appointment.service').then(({ AppointmentService }) => {
      const service = (this as any).store['svc'] as AppointmentService;
      service.getById(id).subscribe({
        next: row => {
          import('../../../appointment/components/form-appointment/form-appointment.component').then(m => {
            this.dialog.open(m.FormAppointmentComponent, {
              width: '600px',
              data: row
            });
            console.log(row);

          });
        },
        error: () => this.sweetAlert.showNotification('Error', 'Establecimiento no encontrado', 'error')
      });
    }
    );
  }

  goHome(): void {
    this.router.navigate(['/']);
  }

  // LOGICA DEL FILTRO

  readonly filterKey = signal<string>('');


  readonly filtered = computed<readonly EstablishmentCard[]>(() => {

    const baseList = (this.selectedPlazaId && this.selectedPlazaId !== 0)
      ? this.filteredEstablishments()
      : this.rows() ?? [];


    let result = [...baseList];

    const q = this.filterKey().trim().toLowerCase();
    if (q) {
      result = result.filter(it =>
        it.name?.toLowerCase().includes(q) ||
        it.description?.toLowerCase().includes(q) ||
        it.address?.toLowerCase().includes(q) ||
        it.rentValueBase?.toString().includes(q)
      );
    }

    const area = this.generalGroup.controls.areaM2.value;
    if (area && area > 0) {
      result = result.filter(it => (it.areaM2 ?? 0) >= area);
    }

    return result;
  });



  onFilterChange(v: string): void {
    this.filterKey.set(v || '');
  }


  // PLAZAS, LOGICA QUE SE ENCARGA DE CARGAR LOS ESTABLECIMIENTOS DEPENDIENDO DE LA PLAZA SELECCIONADA

  private loadPlazas(): void {
    this.squareSvc.getAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => (this.plazas = res ?? [])
      });
  }

  onPlazaChange(plazaId: number | null): void {
    this.selectedPlazaId = plazaId;

    if (!plazaId || plazaId === 0) {
      this.filteredEstablishments.set(this.rows() ?? []);
      this.store.loadAll({ activeOnly: true }); // carga todos los que ya están en el store
      return;
    }

    this.loadingEstablishments = true;
    this.store.clear();

    this.estSvc.getByPlaza(plazaId, { activeOnly: true }).pipe(
      takeUntil(this.destroy$),
      catchError(() => of([])),
      finalize(() => {  
        this.loadingEstablishments = false;
      })
    ).subscribe((list) => {
      this.filteredEstablishments.set(list ?? []);
    });
  }

  // VALIDACIONES AREA M2

  readonly generalGroup = this.fb.group({
    areaM2: this.fb.control(0, {
      validators: [
        Validators.required,
        AppValidators.numberRange({ min: 1, max: 1_000_000 }),
        AppValidators.decimal({ decimals: 2 })
      ]
    }),
  }, { updateOn: 'change' });


  onNumberBlur(control: AbstractControl | null) {
    if (!control) return;
    const v = control.value;
    if (v === null || v === undefined || v === '') return;
    const s = String(v).replace(',', '.');
    const n = Number(s);
    if (!Number.isNaN(n)) control.setValue(n, { emitEvent: false });
    control.updateValueAndValidity({ emitEvent: false });
  }

}
