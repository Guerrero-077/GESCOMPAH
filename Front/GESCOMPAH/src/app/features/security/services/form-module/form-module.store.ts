import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { FormModuleCreateModel, FormModuleSelectModel, FormModuleUpdateModel } from '../../models/form-module..models';
import { FormModuleService } from './form-module.service';

@Injectable({
  providedIn: 'root'
})
export class FormModuleStore {
  private readonly _formModules = new BehaviorSubject<FormModuleSelectModel[]>([]);
  readonly formModules$ = this._formModules.asObservable();

  constructor(private formModuleService: FormModuleService) {
    this.loadAll();
  }

  private get formModules(): FormModuleSelectModel[] {
    return this._formModules.getValue();
  }

  private set formModules(val: FormModuleSelectModel[]) {
    this._formModules.next(val);
  }

  loadAll() {
    this.formModuleService.getAll().pipe(
      tap(data => this.formModules = data),
      catchError(err => {
        console.error('Error loading form modules', err);
        return throwError(() => err);
      })
    ).subscribe();
  }

  getById(id: number): Observable<FormModuleSelectModel> {
    return this.formModuleService.getById(id);
  }

  create(formModule: FormModuleCreateModel): Observable<FormModuleSelectModel> {
    return this.formModuleService.create(formModule).pipe(
      tap(() => {
        this.loadAll(); // Force refresh
      })
    );
  }

  update(id: number, updateDto: FormModuleUpdateModel): Observable<FormModuleSelectModel> {
    return this.formModuleService.update(id, updateDto).pipe(
      tap(() => {
        this.loadAll(); // Force refresh
      })
    );
  }

  delete(id: number): Observable<void> {
    return this.formModuleService.delete(id).pipe(
      tap(() => {
        this.formModules = this.formModules.filter(c => c.id !== id);
      })
    );
  }

  deleteLogic(id: number): Observable<void> {
    return this.formModuleService.deleteLogic(id).pipe(
      tap(() => {
        this.loadAll(); // Force refresh
      })
    );
  }
}
