import { Injectable } from '@angular/core';
import { GenericService } from '../../../../Core/Service/Generic/generic-service';
import { ModulesModule } from '../../Models/module.models';

@Injectable({
  providedIn: 'root'
})
export class ModuleServices extends GenericService<ModulesModule>{

}
