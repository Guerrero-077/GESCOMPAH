import { Injectable } from '@angular/core';
import { GenericService } from '../../../../core/service/generic/generic.service';
import { PersonSelectModel, PersonCreateModel, PersonUpdateModel } from '../../models/person.models';
import { Observable } from 'rxjs';
import { TenantsSelectModel } from '../../../tenants/models/tenants.models';
import { environment } from '../../../../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class PersonService extends GenericService<PersonSelectModel, PersonCreateModel, PersonUpdateModel> {
  protected resource = 'person';

  getByDocument(document: string): Observable<PersonSelectModel | null> {
    return this.http.get<PersonSelectModel | null>(`${environment.apiURL}/${this.resource}/document/${document}`);
  }

}
