import { Injectable } from '@angular/core';
import { GenericService } from '../../../../core/services/generic/generic.service';
import { SquareModel } from '../../models/squares.models';

@Injectable({
  providedIn: 'root'
})
export class SquareService extends GenericService<SquareModel> {

}
