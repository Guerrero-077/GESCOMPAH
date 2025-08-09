import { Injectable } from '@angular/core';
import { ModulesModule } from '../../models/module.models';
import { GenericService } from '../../../../core/service/generic/generic.service';

@Injectable({
  providedIn: 'root'
})
export class ModuleService extends GenericService<ModulesModule>{
  
}
