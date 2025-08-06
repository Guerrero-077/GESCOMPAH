import { Injectable } from '@angular/core';
import { GenericService } from '../../../Core/Service/Generic/generic-service';
import { SquaresModels } from '../Models/squares.models';

@Injectable({
  providedIn: 'root'
})
export class SquaresService extends GenericService<SquaresModels>{
  
}
