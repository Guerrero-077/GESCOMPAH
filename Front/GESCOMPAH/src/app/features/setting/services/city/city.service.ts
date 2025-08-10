import { Injectable } from '@angular/core';
import { GenericService } from '../../../../core/service/generic/generic.service';
import { CityModel } from '../../models/city.models';

@Injectable({
  providedIn: 'root'
})
export class CityService extends GenericService<CityModel> {

}