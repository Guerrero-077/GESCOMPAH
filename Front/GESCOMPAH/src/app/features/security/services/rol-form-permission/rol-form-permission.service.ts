import { Injectable } from '@angular/core';
import { GenericService } from '../../../../core/service/generic/generic.service';
import { RolFormPermissionSelectModel, RolFormPermissionCreateModel, RolFormPermissionUpdateModel } from '../../models/rol-form-permission.models';

@Injectable({
  providedIn: 'root'
})
export class RolFormPermissionService extends GenericService<RolFormPermissionSelectModel, RolFormPermissionCreateModel, RolFormPermissionUpdateModel> {
  protected resource = 'rolFormPermission';

  // You can add any additional methods specific to rol-form-permission service here
  // For example, if you need to fetch permissions by a specific criteria
  // getPermissionsByCriteria(criteria: any): Observable<RolFormPermissionSelectModel[]> {
  //   return this.http.get<RolFormPermissionSelectModel[]>(`${this.baseUrl}/${this.resource}/criteria`, { params: criteria });
  // }
}
