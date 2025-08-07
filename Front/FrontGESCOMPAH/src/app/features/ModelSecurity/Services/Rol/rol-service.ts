import { Injectable } from '@angular/core';
// import { GenericTableComponents } from '../../../../shared/components/generic-table-components/generic-table-components';
import { RolModule } from '../../Models/rol.models';
import { GenericService } from '../../../../Core/Service/Generic/generic-service';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RolService extends GenericService<RolModule> {
  // urlBase = environment.apiURL + '/rol/'
  private rol: RolModule[] = [
    {
      "id": 1,
      "name": "Administrador",
      "description": "usuario con acceso a todas las entidades que no afectan el funcionamiento del sistema",
      "active": true
    },
    {
      "id": 2,
      "name": "Prueba",
      "description": "rol de prueba",
      "active": false
    },
    {
      "id": 3,
      "name": "Arrendatario",
      "description": "usuario que posee un contrato",
      "active": true
    }
  ];

  getAllPruebas(): Observable<RolModule[]>{
    return of(this.rol);
  }
  
}
