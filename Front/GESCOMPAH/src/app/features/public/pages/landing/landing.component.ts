import { CommonModule } from '@angular/common';
import {
  Component,
  DestroyRef,
  ElementRef,
  AfterViewInit,
  inject,
  OnInit,
  ViewChild
} from '@angular/core';
import { RouterLink } from '@angular/router';
import { CardFeatureComponent } from '../../components/card-feature/card-feature.component';
import { CardComponent } from '../../../../shared/components/card/card.component';
import { EstablishmentStore } from '../../../establishments/services/establishment/establishment.store';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatDialog } from '@angular/material/dialog';
import { EstablishmentService } from '../../../establishments/services/establishment/establishment.service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { fromEvent } from 'rxjs';
import { AppointmentStore } from '../../../appointment/services/appointment/appointment.store';
import { AppointmentService } from '../../../appointment/services/appointment/appointment.service';

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [CommonModule, RouterLink, CardFeatureComponent, CardComponent],
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.css']
})
export class LandingComponent implements OnInit, AfterViewInit {
  private readonly store = inject(EstablishmentStore);
  private readonly sweetAlert = inject(SweetAlertService);
  private readonly dialog = inject(MatDialog);
  private readonly destroyRef = inject(DestroyRef);

  readonly items = this.store.items;

  @ViewChild('carrusel', { static: false }) carruselRef!: ElementRef<HTMLDivElement>;

  canScrollLeft = false;
  canScrollRight = false;

  private mutationObserver?: MutationObserver;

  async ngOnInit(): Promise<void> {
    // cargo datos (espera al backend)
    await this.store.loadAll({ limit: 6, activeOnly: true });

    // aseguro que, cuando los datos ya llegaron, intentamos actualizar los botones
    // con un pequeño delay para esperar render del DOM
    setTimeout(() => this.updateScrollButtons(), 50);
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

          });
        }, 
        error: () => this.sweetAlert.showNotification('Error', 'Establecimiento no encontrado', 'error')
      });
    }
    );
  }

  ngAfterViewInit(): void {
    const el = this.carruselRef?.nativeElement;
    if (!el) return;

    // escuchar scroll del contenedor para actualizar botones
    fromEvent(el, 'scroll')
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.updateScrollButtons());

    // resize de ventana (cambia visibilidad)
    fromEvent(window, 'resize')
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.updateScrollButtons());

    // Observer: detecta cuando child nodes (tarjetas) cambian en el contenedor interno
    const list = el.querySelector('.carrusel-locales') as HTMLElement | null;
    if (list) {
      this.mutationObserver = new MutationObserver(() => {
        // cuando cambien nodos (tarjetas añadidas/quitadas), volvemos a calcular botones
        this.updateScrollButtons();
      });
      this.mutationObserver.observe(list, { childList: true, subtree: true });
      // desconectar cuando se destruya el componente
      this.destroyRef.onDestroy(() => this.mutationObserver?.disconnect());
    }

    // primera verificación tras paint
    requestAnimationFrame(() => this.updateScrollButtons());
  }

  private updateScrollButtons(): void {
    const el = this.carruselRef?.nativeElement;
    if (!el) {
      this.canScrollLeft = false;
      this.canScrollRight = false;
      return;
    }
    // actualizando banderas con epsilon para subpixel
    this.canScrollLeft = el.scrollLeft > 5;
    this.canScrollRight = (el.scrollWidth - el.clientWidth - el.scrollLeft) > 5;
  }

  scrollCarrusel(direction: number): void {
    const el = this.carruselRef?.nativeElement;
    if (!el) return;

    // cantidad a desplazar: intenta mover el ancho visible; si quieres 1 tarjeta, ver nota abajo
    const amount = el.clientWidth;
    el.scrollBy({ left: direction * amount, behavior: 'smooth' });

    // actualizo estados unos ms después (para cuando termine la animación)
    setTimeout(() => this.updateScrollButtons(), 300);
  }
}
