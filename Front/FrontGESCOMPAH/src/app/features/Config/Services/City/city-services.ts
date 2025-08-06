import { Injectable } from '@angular/core';
import { GenericService } from '../../../../Core/Service/Generic/generic-service';
import { CityModels } from '../../Models/City.Models';

@Injectable({
  providedIn: 'root'
})
export class CityServices extends GenericService<CityModels> {
  
}
