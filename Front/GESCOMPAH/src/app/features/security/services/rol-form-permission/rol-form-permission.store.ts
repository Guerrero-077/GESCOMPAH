import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { RolFormPermissionCreateModel, RolFormPermissionGroupedModel, RolFormPermissionUpdateModel } from '../../models/rol-form-permission.models';
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

  // Carga inicial (usa el método agrupado del servicio)
  loadAll() {
    this.rolFormPermissionService.getAll().pipe(
      tap(data => this.rolFormPermissions = data),
      catchError(err => {
        console.error('Error loading grouped rolFormPermissions', err);
        return throwError(() => err);
      })
    ).subscribe();
  }

  // Crear/actualizar: refresca la lista agrupada
  create(rolFormPermission: RolFormPermissionCreateModel): Observable<RolFormPermissionGroupedModel> {
    return this.rolFormPermissionService.create(rolFormPermission).pipe(
      tap(() => {
        this.loadAll();
      })
    );
  }

  update(updateDto: RolFormPermissionUpdateModel): Observable<RolFormPermissionGroupedModel> {
    return this.rolFormPermissionService.update(updateDto.id, updateDto).pipe(
      tap(() => {
        this.loadAll();
      })
    );
  }

  // Eliminar por grupo
  deleteByGroup(rolId: number, formId: number): Observable<void> {
    return this.rolFormPermissionService.deleteByGroup(rolId, formId).pipe(
      tap(() => {
        // Después de borrar, recargamos la lista para reflejar los cambios
        this.loadAll();
      })
    );
  }

  // Eliminar individual: también refresca la lista
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

    changeActiveStatus(id: number, active: boolean): Observable<RolFormPermissionGroupedModel> {
      return this.rolFormPermissionService.changeActiveStatus(id, active).pipe(
        tap(() => {
          this.loadAll()
        })
      );
    }
}
