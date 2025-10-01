import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { FinanceService } from './finance.service';
import { DepartmentCreate, DepartmentUpdate } from '../../models/department.models';
import { FinanceCreateModels, FinanceSelectModels, FinanceUpdateModels } from '../../models/finance.models';

@Injectable({
  providedIn: 'root'
})
export class FinanceStore {
  private readonly _finances = new BehaviorSubject<FinanceSelectModels[]>([]);
  readonly finances$ = this._finances.asObservable();

  constructor(private financeService: FinanceService) {
    this.loadAll();
  }

  private get finances(): FinanceSelectModels[] {
    return this._finances.getValue();
  }

  private set finances(val: FinanceSelectModels[]) {
    this._finances.next(val);
  }

  loadAll() {
    this.financeService.getAll().pipe(
      tap(data => this.finances = data),
      catchError(err => {
        console.error('Error loading finances', err);
        return throwError(() => err);
      })
    ).subscribe();
  }

  create(finance: FinanceCreateModels): Observable<FinanceSelectModels> {
    return this.financeService.create(finance as FinanceCreateModels).pipe(
      tap(() => {
        this.loadAll(); // Force refresh
      })
    );
  }

  update(updateDto: FinanceUpdateModels): Observable<FinanceSelectModels> {
    return this.financeService.update(updateDto.id, updateDto).pipe(
      tap(() => {
        this.loadAll(); // Force refresh
      })
    );
  }

  delete(id: number): Observable<void> {
    return this.financeService.delete(id).pipe(
      tap(() => {
        this.finances = this.finances.filter(d => d.id !== id);
      })
    );
  }


  changeActiveStatus(id: number, active: boolean): Observable<FinanceSelectModels> {
    return this.financeService.changeActiveStatus(id, active).pipe(
      tap(() => {
        this.loadAll();
      })
    );
  }
}
