import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RoleCreateModel, RoleSelectModel, RoleUpdateModel } from '../../models/role.models';
import { environment } from '../../../../../environments/environment';
import { GenericService } from '../../../../core/service/generic/generic.service';

@Injectable({
  providedIn: 'root'
})
export class RoleService extends GenericService<RoleSelectModel, RoleCreateModel, RoleUpdateModel> {
  protected resource = 'rol';
  // You can add any additional methods specific to role service here
  // For example, if you need to fetch roles by a specific criteria
  // getRolesByCriteria(criteria: any): Observable<RoleSelectModel[]> {
  //   return this.http.get<RoleSelectModel[]>(`${this.baseUrl}/${this.resource}/criteria`, { params: criteria });
  // }
}
