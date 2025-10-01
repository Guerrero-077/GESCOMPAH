import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { RoleCreateModel, RoleSelectModel, RoleUpdateModel } from '../../models/role.models';
import { RoleService } from './role.service';
import { IsActive } from '../../../../core/models/IsAcitve.models';

@Injectable({
  providedIn: 'root'
})
export class RoleStore {
  private readonly _roles = new BehaviorSubject<RoleSelectModel[]>([]);
  readonly roles$ = this._roles.asObservable();

  constructor(private roleService: RoleService) {
    this.loadAll();
  }

  private get roles(): RoleSelectModel[] {
    return this._roles.getValue();
  }

  private set roles(val: RoleSelectModel[]) {
    this._roles.next(val);
  }

  loadAll() {
    this.roleService.getAll().pipe(
      tap(data => this.roles = data),
      catchError(err => {
        console.error('Error loading roles', err);
        return throwError(() => err);
      })
    ).subscribe();
  }

  create(role: RoleCreateModel): Observable<RoleSelectModel> {
    return this.roleService.create(role).pipe(
      tap(() => {
        this.loadAll(); // Force refresh
      })
    );
  }

  update(updateDto: RoleUpdateModel): Observable<RoleSelectModel> {
    return this.roleService.update(updateDto.id, updateDto).pipe(
      tap(() => {
        this.loadAll(); // Force refresh
      })
    );
  }

  delete(id: number): Observable<void> {
    return this.roleService.delete(id).pipe(
      tap(() => {
        this.roles = this.roles.filter(c => c.id !== id);
      })
    );
  }

  deleteLogic(id: number): Observable<void> {
    return this.roleService.deleteLogic(id).pipe(
      tap(() => {
        this.loadAll(); // Force refresh
      })
    );
  }

  changeActiveStatus(id: number, active: boolean): Observable<IsActive> {
    return this.roleService.changeActiveStatus(id, active).pipe(
      tap(() => {
        this.loadAll();
      })
    );
  }
}
