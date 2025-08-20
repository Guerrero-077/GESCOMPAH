import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { PermissionCreateModel, PermissionSelectModel, PermissionUpdateModel } from '../../models/permission.models';
import { PermissionService } from './permission.service';

@Injectable({
  providedIn: 'root'
})
export class PermissionStore {
  private readonly _permissions = new BehaviorSubject<PermissionSelectModel[]>([]);
  readonly permissions$ = this._permissions.asObservable();

  constructor(private permissionService: PermissionService) {
    this.loadAll();
  }

  private get permissions(): PermissionSelectModel[] {
    return this._permissions.getValue();
  }

  private set permissions(val: PermissionSelectModel[]) {
    this._permissions.next(val);
  }

  loadAll() {
    this.permissionService.getAll().pipe(
      tap(data => this.permissions = data),
      catchError(err => {
        console.error('Error loading permissions', err);
        return throwError(() => err);
      })
    ).subscribe();
  }

  create(permission: PermissionCreateModel): Observable<PermissionSelectModel> {
    return this.permissionService.create(permission).pipe(
      tap(() => {
        this.loadAll(); // Force refresh
      })
    );
  }

  update(updateDto: PermissionUpdateModel): Observable<PermissionSelectModel> {
    return this.permissionService.update(updateDto.id, updateDto).pipe(
      tap(() => {
        this.loadAll(); // Force refresh
      })
    );
  }

  delete(id: number): Observable<void> {
    return this.permissionService.delete(id).pipe(
      tap(() => {
        this.permissions = this.permissions.filter(c => c.id !== id);
      })
    );
  }

  deleteLogic(id: number): Observable<void> {
    return this.permissionService.deleteLogic(id).pipe(
      tap(() => {
        this.loadAll(); // Force refresh
      })
    );
  }

  changeActiveStatus(id: number, active: boolean): Observable<PermissionSelectModel> {
    return this.permissionService.changeActiveStatus(id, active).pipe(
      tap(() => {
        this.loadAll(); // Force refresh
      })
    );
  }
}
