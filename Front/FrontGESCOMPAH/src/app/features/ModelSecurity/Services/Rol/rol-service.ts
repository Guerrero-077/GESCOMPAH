import { Injectable } from '@angular/core';
import { GenericTableComponents } from '../../../../shared/components/generic-table-components/generic-table-components';
import { RolModule } from '../../Models/rol.models';
import { GenericService } from '../../../../Core/Service/Generic/generic-service';

@Injectable({
  providedIn: 'root'
})
export class RolService extends GenericService<RolModule> {
  // urlBase = environment.apiURL + '/rol/'

}
