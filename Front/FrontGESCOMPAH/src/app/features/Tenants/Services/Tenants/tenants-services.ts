import { Injectable } from '@angular/core';
import { GenericService } from '../../../../Core/Service/Generic/generic-service';
import { TenantsModels } from '../../Models/Tenants.mode';

@Injectable({
  providedIn: 'root'
})
export class TenantsServices extends GenericService<TenantsModels> {

}
