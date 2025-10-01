import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { DepartmentSelectModel, DepartmentCreate, DepartmentUpdate } from '../../models/department.models';
import { DepartmentService } from './department.service';

@Injectable({
  providedIn: 'root'
})
export class DepartmentStore {
  private readonly _departments = new BehaviorSubject<DepartmentSelectModel[]>([]);
  readonly departments$ = this._departments.asObservable();

  constructor(private departmentService: DepartmentService) {
    this.loadAll();
  }

  private get departments(): DepartmentSelectModel[] {
    return this._departments.getValue();
  }

  private set departments(val: DepartmentSelectModel[]) {
    this._departments.next(val);
  }

  // Carga inicial
  loadAll() {
    this.departmentService.getAll().pipe(
      tap(data => this.departments = data),
      catchError(err => {
        console.error('Error loading departments', err);
        return throwError(() => err);
      })
    ).subscribe();
  }

  create(department: DepartmentCreate): Observable<DepartmentSelectModel> {
    return this.departmentService.create(department as DepartmentSelectModel).pipe(
      tap(() => this.loadAll())
    );
  }

  update(updateDto: DepartmentUpdate): Observable<DepartmentSelectModel> {
    return this.departmentService.update(updateDto.id, updateDto).pipe(
      tap(() => this.loadAll())
    );
  }

  delete(id: number): Observable<void> {
    return this.departmentService.delete(id).pipe(
      tap(() => {
        this.departments = this.departments.filter(d => d.id !== id);
      })
    );
  }


  changeActiveStatus(id: number, active: boolean): Observable<DepartmentSelectModel> {
    return this.departmentService.changeActiveStatus(id, active).pipe(
      tap(() => {
        this.loadAll();
      })
    );
  }
}
