import { Injectable } from '@angular/core';
import { GenericService } from '../../../../Core/Service/Generic/generic-service';
import { PermissionModule } from '../../Models/Permission.Models';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PermissionServices extends GenericService<PermissionModule> {
  private permission: PermissionModule[] = [
    {
      "id": 1,
      "name": "Editar",
      "description": "permiso de editar las entidades",
      "active": true
    },
    {
      "id": 2,
      "name": "Eliminar",
      "description": "permiso de eliminar las entidades",
      "active": true
    },
    {
      "id": 3,
      "name": "Crear",
      "description": "permiso de crear las entidades",
      "active": true
    },
  ];

  getAllPruebas(): Observable<PermissionModule[]>{
    return of(this.permission);
  }
}
