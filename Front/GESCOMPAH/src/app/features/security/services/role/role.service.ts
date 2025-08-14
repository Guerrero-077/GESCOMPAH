import { Injectable } from '@angular/core';
import { GenericService } from '../../../../core/service/generic/generic.service';
import { RoleCreateModel, RoleSelectModel, RoleUpdateModel } from '../../models/role.models';

@Injectable({
  providedIn: 'root'
})
export class RoleService extends GenericService<RoleSelectModel, RoleCreateModel, RoleUpdateModel> {
  protected resource = 'rol';

  // You can add any additional methods specific to role service here
  // For example, if you need to fetch roles by a specific criteria
  // getRolesByCriteria(criteria: any): Observable<RoleSelectModule[]> {
  //   return this.http.get<RoleSelectModule[]>(`${this.baseUrl}/${this.resource}/criteria`, { params: criteria });
  // }

}
