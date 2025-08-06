import { Injectable } from '@angular/core';
import { GenericService } from '../../../../Core/Service/Generic/generic-service';
import { ModulesModule } from '../../Models/module.models';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ModuleServices extends GenericService<ModulesModule>{
  private module: ModulesModule[] = [
    {
      "id": 1,
      "name": "Administracion",
      "description": "Modulo de gestion",
      "icon": "shield-lock",
      "active": true
    },
    {
      "id": 2,
      "name": "Arrendatario",
      "description": "Modulo de gestion",
      "icon": "shield-lock",
      "active": true
    }
  ];

  getAllPruebas(): Observable<ModulesModule[]> {
    return of(this.module);
  }

}
