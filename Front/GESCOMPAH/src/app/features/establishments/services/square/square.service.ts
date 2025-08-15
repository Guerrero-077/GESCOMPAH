import { Injectable } from '@angular/core';
import { GenericService } from '../../../../core/service/generic/generic.service';
import { SquareCreateModel, SquareSelectModel, SquareUpdateModel } from '../../models/squares.models';

@Injectable({
  providedIn: 'root'
})
export class SquareService extends GenericService<SquareSelectModel, SquareCreateModel, SquareUpdateModel> {
  protected resource = 'plaza';

  // You can add any additional methods specific to square service here
  // For example, if you need to fetch squares by a specific criteria
  // getSquaresByCriteria(criteria: any): Observable<SquareSelectModel[]> {
  //   return this.http.get<SquareSelectModel[]>(`${this.baseUrl}/${this.resource}/criteria`, { params: criteria });
  // }

}
