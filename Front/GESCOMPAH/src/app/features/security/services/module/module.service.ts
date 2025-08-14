import { Injectable } from '@angular/core';
import { GenericService } from '../../../../core/service/generic/generic.service';
import { ModuleCreateModel, ModuleSelectModel, ModuleUpdateModel } from '../../models/module.models';

@Injectable({
  providedIn: 'root'
})
export class ModuleService extends GenericService<ModuleSelectModel, ModuleCreateModel, ModuleUpdateModel> {
  protected resource = 'module';

  // You can add any additional methods specific to module service here
  // For example, if you need to fetch modules by a specific criteria
  // getModulesByCriteria(criteria: any): Observable<ModuleSelectModule[]> {
  //   return this.http.get<ModuleSelectModule[]>(`${this.baseUrl}/${this.resource}/criteria`, { params: criteria });
  // }

}
