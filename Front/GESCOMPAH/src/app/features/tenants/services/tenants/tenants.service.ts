import { Injectable } from '@angular/core';
import { GenericService } from '../../../../core/service/generic/generic.service';
import { TenantsCreateModel, TenantsSelectModel, TenantsUpdateModel } from '../../models/tenants.models';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TenantsService extends GenericService<TenantsSelectModel, TenantsCreateModel, TenantsUpdateModel> {
  protected resource = 'user';

}
