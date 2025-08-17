import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { RolFormPermissionGroupedModel, RolFormPermissionCreateModel, RolFormPermissionUpdateModel, RolFormPermissionSelectModel } from '../../models/rol-form-permission.models';
import { RolFormPermissionService } from './rol-form-permission.service';

@Injectable({
  providedIn: 'root'
})
export class RolFormPermissionStore {
  // El estado ahora guarda el modelo agrupado
  private readonly _rolFormPermissions = new BehaviorSubject<RolFormPermissionGroupedModel[]>([]);
  readonly rolFormPermissions$ = this._rolFormPermissions.asObservable();

  constructor(private rolFormPermissionService: RolFormPermissionService) {
    this.loadAll();
  }

  private get rolFormPermissions(): RolFormPermissionGroupedModel[] {
    return this._rolFormPermissions.getValue();
  }

  private set rolFormPermissions(val: RolFormPermissionGroupedModel[]) {
    this._rolFormPermissions.next(val);
  }

  // loadAll ahora usa el nuevo método del servicio
  loadAll() {
    this.rolFormPermissionService.getAllGrouped().pipe(
      tap(data => this.rolFormPermissions = data),
      catchError(err => {
        console.error('Error loading grouped rolFormPermissions', err);
        return throwError(() => err);
      })
    ).subscribe();
  }

  // create y update ahora refrescan la lista agrupada
  create(rolFormPermission: RolFormPermissionCreateModel): Observable<RolFormPermissionSelectModel> {
    return this.rolFormPermissionService.create(rolFormPermission).pipe(
      tap(() => {
        this.loadAll();
      })
    );
  }

  update(updateDto: RolFormPermissionUpdateModel): Observable<RolFormPermissionSelectModel> {
    return this.rolFormPermissionService.update(updateDto.id, updateDto).pipe(
      tap(() => {
        this.loadAll();
      })
    );
  }

  // --- MÉTODO NUEVO Y CORREGIDO ---
  deleteByGroup(rolId: number, formId: number): Observable<void> {
    return this.rolFormPermissionService.deleteByGroup(rolId, formId).pipe(
      tap(() => {
        // Después de borrar, recargamos la lista para reflejar los cambios
        this.loadAll();
      })
    );
  }

  // Dejamos los métodos de borrado individual por si se usan en otra parte,
  // pero ahora también refrescan la lista principal.
  delete(id: number): Observable<void> {
    return this.rolFormPermissionService.delete(id).pipe(
      tap(() => {
        this.loadAll();
      })
    );
  }

  deleteLogic(id: number): Observable<void> {
    return this.rolFormPermissionService.deleteLogic(id).pipe(
      tap(() => {
        this.loadAll();
      })
    );
  }
}