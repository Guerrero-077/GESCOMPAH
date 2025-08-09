import { Injectable } from '@angular/core';
import { GenericService } from '../../../../core/services/generic/generic.service';
import { RoleModule } from '../../models/role.models';

@Injectable({
  providedIn: 'root'
})
export class RoleService extends GenericService<RoleModule> {

}
