import { Injectable } from '@angular/core';
import { GenericService } from '../../../../Core/Service/Generic/generic-service';
import { PermissionModule } from '../../Models/Permission.Models';

@Injectable({
  providedIn: 'root'
})
export class PermissionServices extends GenericService<PermissionModule> {

}
