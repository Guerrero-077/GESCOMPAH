import { Injectable } from '@angular/core';
import { GenericService } from '../../../../core/service/generic/generic.service';
import { ModuleCreateModel, ModuleSelectModel, ModuleUpdateModel } from '../../models/module.models';

@Injectable({
  providedIn: 'root'
})
export class ModuleService extends GenericService<ModuleSelectModel, ModuleCreateModel, ModuleUpdateModel> {
  protected override resource = 'module';
} {
}
