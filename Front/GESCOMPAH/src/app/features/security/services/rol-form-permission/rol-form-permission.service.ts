import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { GenericService } from '../../../../core/service/generic/generic.service';
import { RolFormPermissionCreateModel, RolFormPermissionGroupedModel, RolFormPermissionUpdateModel } from '../../models/rol-form-permission.models';

@Injectable({
  providedIn: 'root'
})
export class RolFormPermissionService extends GenericService<RolFormPermissionGroupedModel, RolFormPermissionCreateModel, RolFormPermissionUpdateModel> {
  protected override resource = 'rolformpermission';

  // Métodos específicos
  override getAll(): Observable<RolFormPermissionGroupedModel[]> {
    return this.http.get<RolFormPermissionGroupedModel[]>(this.url('grouped'));
  }

  deleteByGroup(rolId: number, formId: number): Observable<void> {
    return this.http.delete<void>(this.url('group', rolId, formId));
  }

}
