import { Injectable } from '@angular/core';
import { GenericService } from '../../../../core/service/generic/generic.service';
import { FormModuleSelectModel, FormModuleCreateModel, FormModuleUpdateModel } from '../../models/form-module..models';

@Injectable({
  providedIn: 'root'
})
export class FormModuleService extends GenericService<FormModuleSelectModel, FormModuleCreateModel, FormModuleUpdateModel> {
  protected resource = 'formModule';

  // You can add any additional methods specific to form module service here
  // For example, if you need to fetch form modules by a specific criteria
  // getFormModulesByCriteria(criteria: any): Observable<FormModuleSelectModel[]> {
  //   return this.http.get<FormModuleSelectModel[]>(`${this.baseUrl}/${this.resource}/criteria`, { params: criteria });
  // }
}
