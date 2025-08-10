import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { DepartmentModel, DepartmentCreate, DepartmentUpdate } from '../../models/department.models';
import { DepartmentService } from './department.service';

@Injectable({
  providedIn: 'root'
})
export class DepartmentStore {
  private readonly _departments = new BehaviorSubject<DepartmentModel[]>([]);
  readonly departments$ = this._departments.asObservable();

  constructor(private departmentService: DepartmentService) {
    this.loadAll();
  }

  private get departments(): DepartmentModel[] {
    return this._departments.getValue();
  }

  private set departments(val: DepartmentModel[]) {
    this._departments.next(val);
  }

  loadAll() {
    this.departmentService.getAll("Department").pipe(
      tap(data => this.departments = data),
      catchError(err => {
        console.error('Error loading departments', err);
        return throwError(() => err);
      })
    ).subscribe();
  }

  create(department: DepartmentCreate): Observable<DepartmentModel> {
    return this.departmentService.Add("Department", department as DepartmentModel).pipe(
      tap(() => {
        this.loadAll(); // Force refresh
      })
    );
  }

  update(updateDto: DepartmentUpdate): Observable<DepartmentModel> {
    return this.departmentService.Update("Department", updateDto.id, updateDto).pipe(
      tap(() => {
        this.loadAll(); // Force refresh
      })
    );
  }

  delete(id: number): Observable<DepartmentModel> {
    return this.departmentService.Delete("Department", id).pipe(
      tap(() => {
        this.departments = this.departments.filter(d => d.id !== id);
      })
    );
  }
}
