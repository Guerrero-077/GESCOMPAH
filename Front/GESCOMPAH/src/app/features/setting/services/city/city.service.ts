import { Injectable } from '@angular/core';
import { GenericService } from '../../../../core/service/generic/generic.service';
import { CityCreate, CitySelectModel, CityUpdate } from '../../models/city.models';

@Injectable({
  providedIn: 'root'
})
export class CityService extends GenericService<CitySelectModel, CityCreate, CityUpdate> {
  protected resource = 'city';
  // getCitiesByDepartment(departmentId: number): Observable<CityModel[]> {
  //   return this.http.get<CityModel[]>(`${this.apiUrl}/city/by-department/${departmentId}`);
  // }
}
