import { Injectable } from '@angular/core';
import { GenericService } from '../../../../core/service/generic/generic.service';
import { DepartmentCreate, DepartmentSelectModel, DepartmentUpdate } from '../../models/department.models';

@Injectable({
  providedIn: 'root'
})
export class DepartmentService extends GenericService<DepartmentSelectModel, DepartmentCreate, DepartmentUpdate> {
  protected resource = 'department';

  // You can add any additional methods specific to department service here
  // For example, if you need to fetch departments by a specific criteria
  // getDepartmentsByCriteria(criteria: any): Observable<DepartmentSelectModel[]> {
  //   return this.http.get<DepartmentSelectModel[]>(`${this.baseUrl}/${this.resource}/criteria`, { params: criteria });
  // }

}
