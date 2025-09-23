import { Injectable } from '@angular/core';
import { GenericService } from '../../../../core/service/generic/generic.service';
import { FormModuleCreateModel, FormModuleSelectModel, FormModuleUpdateModel } from '../../models/form-module.model';

@Injectable({
  providedIn: 'root'
})
export class FormModuleService extends GenericService<FormModuleSelectModel, FormModuleCreateModel, FormModuleUpdateModel> {
  protected override resource = 'formModule';

}
