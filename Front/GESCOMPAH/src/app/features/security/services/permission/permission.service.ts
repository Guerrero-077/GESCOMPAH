import { Injectable } from '@angular/core';
import { PermissionModule } from '../../models/permission.models';
import { GenericService } from '../../../../core/service/generic/generic.service';

@Injectable({
  providedIn: 'root'
})
export class PermissionService extends GenericService<PermissionModule>{
  
}
