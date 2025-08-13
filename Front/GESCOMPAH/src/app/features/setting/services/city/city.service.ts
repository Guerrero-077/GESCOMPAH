import { Injectable } from '@angular/core';
import { GenericService } from '../../../../core/service/generic/generic.service';
import { CityModel } from '../../models/city.models';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CityService extends GenericService<CityModel> {
  // getCitiesByDepartment(departmentId: number): Observable<CityModel[]> {
  //   return this.http.get<CityModel[]>(`${this.apiUrl}/city/by-department/${departmentId}`);
  // }
}
