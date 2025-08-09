import { Injectable } from '@angular/core';
import { RoleModule } from '../../models/role.models';
import { GenericService } from '../../../../core/service/generic/generic.service';

@Injectable({
  providedIn: 'root'
})
export class RoleService extends GenericService<RoleModule> {

}
