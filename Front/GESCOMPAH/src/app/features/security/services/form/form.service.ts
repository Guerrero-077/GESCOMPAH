import { Injectable } from '@angular/core';
import { GenericService } from '../../../../core/service/generic/generic.service';
import { FormCreateModel, FormSelectModel, FormUpdateModel } from '../../models/form.models';

@Injectable({
  providedIn: 'root'
})
export class FormService extends GenericService<FormSelectModel, FormCreateModel, FormUpdateModel> {
  protected resource = 'form';

  // You can add any additional methods specific to form service here
  // For example, if you need to fetch forms by a specific criteria
  // getFormsByCriteria(criteria: any): Observable<FormSelectModule[]> {
  //   return this.http.get<FormSelectModule[]>(`${this.baseUrl}/${this.resource}/criteria`, { params: criteria });
  // }

}
