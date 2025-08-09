import { Injectable } from '@angular/core';
import { GenericService } from '../../../../core/services/generic/generic.service';
import { ModulesModule } from '../../models/module.models';

@Injectable({
  providedIn: 'root'
})
export class ModuleService extends GenericService<ModulesModule>{
  
}
