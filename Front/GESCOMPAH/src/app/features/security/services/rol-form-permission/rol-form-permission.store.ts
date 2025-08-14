import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { RolFormPermissionSelectModel, RolFormPermissionCreateModel, RolFormPermissionUpdateModel } from '../../models/rol-form-permission.models';
import { RolFormPermissionService } from './rol-form-permission.service';

@Injectable({
  providedIn: 'root'
})
export class RolFormPermissionStore {
  private readonly _rolFormPermissions = new BehaviorSubject<RolFormPermissionSelectModel[]>([]);
  readonly rolFormPermissions$ = this._rolFormPermissions.asObservable();

  constructor(private rolFormPermissionService: RolFormPermissionService) {
    this.loadAll();
  }

  private get rolFormPermissions(): RolFormPermissionSelectModel[] {
    return this._rolFormPermissions.getValue();
  }

  private set rolFormPermissions(val: RolFormPermissionSelectModel[]) {
    this._rolFormPermissions.next(val);
  }

  loadAll() {
    this.rolFormPermissionService.getAll().pipe(
      tap(data => this.rolFormPermissions = data),
      catchError(err => {
        console.error('Error loading rolFormPermissions', err);
        return throwError(() => err);
      })
    ).subscribe();
  }

  create(rolFormPermission: RolFormPermissionCreateModel): Observable<RolFormPermissionSelectModel> {
    return this.rolFormPermissionService.create(rolFormPermission).pipe(
      tap(() => {
        this.loadAll(); // Force refresh
      })
    );
  }

  update(updateDto: RolFormPermissionUpdateModel): Observable<RolFormPermissionSelectModel> {
    return this.rolFormPermissionService.update(updateDto.id, updateDto).pipe(
      tap(() => {
        this.loadAll(); // Force refresh
      })
    );
  }

  delete(id: number): Observable<void> {
    return this.rolFormPermissionService.delete(id).pipe(
      tap(() => {
        this.rolFormPermissions = this.rolFormPermissions.filter(c => c.id !== id);
      })
    );
  }

  deleteLogic(id: number): Observable<void> {
    return this.rolFormPermissionService.deleteLogic(id).pipe(
      tap(() => {
        this.rolFormPermissions = this.rolFormPermissions.filter(c => c.id !== id);
      })
    );
  }
}
