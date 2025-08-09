import { Injectable } from '@angular/core';
import { SquareModel } from '../../models/squares.models';
import { GenericService } from '../../../../core/service/generic/generic.service';

@Injectable({
  providedIn: 'root'
})
export class SquareService extends GenericService<SquareModel> {

}
