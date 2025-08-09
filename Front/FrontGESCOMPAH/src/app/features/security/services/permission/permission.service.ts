import { Injectable } from '@angular/core';
import { GenericService } from '../../../../core/services/generic/generic.service';
import { PermissionModule } from '../../models/permission.models';

@Injectable({
  providedIn: 'root'
})
export class PermissionService extends GenericService<PermissionModule>{
  
}
