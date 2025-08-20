// services/user.service.ts
import { Injectable } from '@angular/core';
import { UserCreateModel, UserSelectModel, UserUpdateModel } from '../../models/user.models';
import { GenericService } from '../../../../core/service/generic/generic.service';

@Injectable({ providedIn: 'root' })
export class UserService extends GenericService<UserSelectModel, UserCreateModel, UserUpdateModel> {
  protected resource = 'user';
  // You can add any additional methods specific to user service here
  // For example, if you need to fetch users by a specific criteria
  // getUsersByCriteria(criteria: any): Observable<UserListDto[]> {
  //   return this.http.get<UserListDto[]>(`${this.baseUrl}/${this.resource}/criteria`, { params: criteria });
  // }
}
