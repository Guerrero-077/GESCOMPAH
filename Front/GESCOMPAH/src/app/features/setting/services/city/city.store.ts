import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { CityCreate, CitySelectModel, CityUpdate } from '../../models/city.models';
import { CityService } from './city.service';

@Injectable({
  providedIn: 'root'
})
export class CityStore {
  private readonly _cities = new BehaviorSubject<CitySelectModel[]>([]);
  readonly cities$ = this._cities.asObservable();

  constructor(private cityService: CityService) {
    this.loadAll();
  }

  private get cities(): CitySelectModel[] {
    return this._cities.getValue();
  }

  private set cities(val: CitySelectModel[]) {
    this._cities.next(val);
  }

  // Carga inicial
  loadAll() {
    this.cityService.getAll().pipe(
      tap(data => this.cities = data),
      catchError(err => {
        console.error('Error loading cities', err);
        return throwError(() => err);
      })
    ).subscribe();
  }
  // Cargar por departamento
  loadByDepartment(departmentId: number) {
    this.cityService.getCitiesByDepartment(departmentId).pipe(
      tap(data => this.cities = data),
      catchError(err => {
        console.error('Error loading cities by department', err);
        return throwError(() => err);
      })
    ).subscribe();
  }

  create(city: CityCreate): Observable<CitySelectModel> {
    return this.cityService.create(city).pipe(
      tap(() => this.loadAll())
    );
  }

  update(updateDto: CityUpdate): Observable<CitySelectModel> {
    return this.cityService.update(updateDto.id, updateDto).pipe(
      tap(() => this.loadAll())
    );
  }

  delete(id: number): Observable<void> {
    return this.cityService.delete(id).pipe(
      tap(() => {
        this.cities = this.cities.filter(c => c.id !== id);
      })
    );
  }

  changeActiveStatus(id: number, active: boolean): Observable<CitySelectModel> {
    return this.cityService.changeActiveStatus(id, active).pipe(
      tap(() => {
        this.loadAll();
      })
    );
  }
}
