import { Injectable } from '@angular/core';
import { GenericService } from '../../../../core/services/generic/generic.service';
import { TenantsModel } from '../../models/tenants.models';

@Injectable({
  providedIn: 'root'
})
export class TenantsService extends GenericService<TenantsModel> {

}
