import { Injectable } from '@angular/core';
import { GenericService } from '../../../../core/service/generic/generic.service';
import { CityCreate, CitySelectModel, CityUpdate } from '../../models/city.models';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CityService extends GenericService<CitySelectModel, CityCreate, CityUpdate> {
  protected resource = 'city';
  getCitiesByDepartment(departmentId: number): Observable<CitySelectModel[]> {
    return this.http.get<CitySelectModel[]>(
      `${this.baseUrl}/${this.resource}/CityWithDepartment/${departmentId}`
    );
  }
}
