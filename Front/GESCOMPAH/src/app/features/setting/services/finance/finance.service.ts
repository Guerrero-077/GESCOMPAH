import { Injectable } from '@angular/core';
import { GenericService } from '../../../../core/service/generic/generic.service';
import { FinanceCreateModels, FinanceSelectModels, FinanceUpdateModels } from '../../models/finance.models';

@Injectable({
  providedIn: 'root'
})
export class FinanceService extends GenericService<FinanceSelectModels, FinanceCreateModels, FinanceUpdateModels> {
  protected override resource = 'SystemParameter';

}
