import { Injectable } from '@angular/core';
import { TenantsModel } from '../../models/tenants.models';
import { GenericService } from '../../../../core/service/generic/generic.service';

@Injectable({
  providedIn: 'root'
})
export class TenantsService extends GenericService<TenantsModel> {

}
