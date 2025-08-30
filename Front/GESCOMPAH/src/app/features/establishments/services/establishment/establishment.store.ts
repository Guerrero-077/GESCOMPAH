import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { EstablishmentSelect, EstablishmentCreate, EstablishmentUpdate, ImageSelectDto } from '../../models/establishment.models';
import { EstablishmentService } from './establishment.service';

@Injectable({ providedIn: 'root' })
export class EstablishmentStore {
  private readonly _establishments = new BehaviorSubject<EstablishmentSelect[]>([]);
  readonly establishments$ = this._establishments.asObservable();

  /** Cambia este flag según la pantalla: true = solo activos, false = todos */
  private readonly activeOnlyView = false;

  constructor(private establishmentService: EstablishmentService) {
    this.loadAll(); // Carga inicial
  }

  private get establishments(): EstablishmentSelect[] { return this._establishments.getValue(); }
  private set establishments(val: EstablishmentSelect[]) { this._establishments.next(val); }

  loadAll(): void {
    const obs = this.activeOnlyView
      ? this.establishmentService.getAllActive()
      : this.establishmentService.getAllAny();

    obs.pipe(
      tap(data => this.establishments = data),
      catchError(err => {
        console.error('Error loading establishments', err);
        return throwError(() => err);
      })
    ).subscribe();
  }

  create(establishment: EstablishmentCreate): Observable<EstablishmentSelect> {
    return this.establishmentService.create(establishment).pipe(
      tap(newEst => {
        if (this.activeOnlyView && !newEst.active) return;
        this.establishments = [...this.establishments, newEst];
      })
    );
  }

  update(updateDto: EstablishmentUpdate): Observable<EstablishmentSelect> {
    return this.establishmentService.update(updateDto).pipe(
      tap(updated => {
        const idx = this.establishments.findIndex(e => e.id === updated.id);

        if (this.activeOnlyView) {
          // Si ahora quedó inactivo, quítalo de la lista visible
          if (!updated.active) {
            this.establishments = this.establishments.filter(e => e.id !== updated.id);
            return;
          }
        }

        if (idx > -1) {
          const copy = [...this.establishments];
          copy[idx] = updated;
          this.establishments = copy;
        } else {
          if (!this.activeOnlyView || updated.active) {
            this.establishments = [...this.establishments, updated];
          }
        }
      })
    );
  }

  delete(id: number): Observable<void> {
    return this.establishmentService.delete(id).pipe(
      tap(() => {
        this.establishments = this.establishments.filter(e => e.id !== id);
      })
    );
  }

  applyImages(establishmentId: number, images: ImageSelectDto[]): void {
    const next = this.establishments.map(e =>
      e.id === establishmentId ? { ...e, images: [...(e.images ?? []), ...images] } : e
    );
    this.establishments = next;
  }

  removeMany(ids: number[]): void {
    const set = new Set(ids);
    this.establishments = this.establishments.filter(e => !set.has(e.id));
  }

  patchActiveMany(ids: number[], active: boolean): void {
    if (!ids?.length) return;
    const set = new Set(ids);
    const next = this.establishments
      .map(e => set.has(e.id) ? { ...e, active } : e)
      // Si la vista es "solo activos", elimina los que quedaron inactivos
      .filter(e => !this.activeOnlyView || e.active);
    this.establishments = next;
  }

  patchActiveOne(id: number, active: boolean): void {
    const next = this.establishments
      .map(e => e.id === id ? { ...e, active } : e)
      .filter(e => !this.activeOnlyView || e.active);
    this.establishments = next;
  }
}
