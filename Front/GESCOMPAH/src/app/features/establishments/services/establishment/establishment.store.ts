import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { EstablishmentSelect, EstablishmentCreate, EstablishmentUpdate, ImageSelectDto } from '../../models/establishment.models';
import { EstablishmentService } from './establishment.service';

@Injectable({
  providedIn: 'root'
})
export class EstablishmentStore {
  private readonly _establishments = new BehaviorSubject<EstablishmentSelect[]>([]);
  readonly establishments$ = this._establishments.asObservable();

  constructor(private establishmentService: EstablishmentService) {
    this.loadAll(); // Carga inicial
  }

  private get establishments(): EstablishmentSelect[] {
    return this._establishments.getValue();
  }

  private set establishments(val: EstablishmentSelect[]) {
    this._establishments.next(val);
  }

  loadAll() {
    this.establishmentService.getAll().pipe(
      tap(data => this.establishments = data),
      catchError(err => {
        console.error('Error loading establishments', err);
        return throwError(() => err);
      })
    ).subscribe();
  }

  create(establishment: EstablishmentCreate): Observable<EstablishmentSelect> {
    return this.establishmentService.create(establishment).pipe(
      tap(newEstablishment => {
        this.establishments = [...this.establishments, newEstablishment];
      })
    );
  }

  update(updateDto: EstablishmentUpdate): Observable<EstablishmentSelect> {
    return this.establishmentService.update(updateDto).pipe(
      tap(updatedEstablishment => {
        const index = this.establishments.findIndex(e => e.id === updatedEstablishment.id);
        if (index > -1) {
          const newList = [...this.establishments];
          newList[index] = updatedEstablishment;
          this.establishments = newList;
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
      e.id === establishmentId
        ? { ...e, images: [...(e.images ?? []), ...images] }
        : e
    );
    this.establishments = next;
  }
}

