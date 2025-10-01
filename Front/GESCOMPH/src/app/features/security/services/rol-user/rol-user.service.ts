import { Injectable } from '@angular/core';
import { GenericService } from '../../../../core/service/generic/generic.service';
import { RolUserCreateModel, RolUserSelectModel, RolUserUpdateModel } from '../../models/rol-user.models';

@Injectable({
  providedIn: 'root'
})
export class RolUserService extends GenericService<RolUserSelectModel, RolUserCreateModel, RolUserUpdateModel> {
  protected resource = 'rolUser';

  // You can add any additional methods specific to rol-user service here
  // For example, if you need to fetch rol-users by a specific criteria
  // getRolUsersByCriteria(criteria: any): Observable<RolUserSelectModel[]> {
  //   return this.http.get<RolUserSelectModel[]>(`${this.baseUrl}/${this.resource}/criteria`, { params: criteria });
  // }
}
