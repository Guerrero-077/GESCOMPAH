import { Injectable } from '@angular/core';
import { GenericService } from '../../../../core/service/generic/generic.service';
import { PermissionCreateModel, PermissionSelectModel, PermissionUpdateModel } from '../../models/permission.models';

@Injectable({
  providedIn: 'root'
})
export class PermissionService extends GenericService<PermissionSelectModel, PermissionCreateModel, PermissionUpdateModel> {
  protected resource = 'permission';

  // You can add any additional methods specific to permission service here
  // For example, if you need to fetch permissions by a specific criteria
  // getPermissionsByCriteria(criteria: any): Observable<PermissionSelectModule[]> {
  //   return this.http.get<PermissionSelectModule[]>(`${this.baseUrl}/${this.resource}/criteria`, { params: criteria });
  // }

}
