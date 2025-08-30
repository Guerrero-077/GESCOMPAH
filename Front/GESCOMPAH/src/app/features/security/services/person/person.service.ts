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

  // You can add any additional methods specific to person service here
  // For example, if you need to fetch persons by a specific criteria
  // getPersonsByCriteria(criteria: any): Observable<PersonSelectModel[]> {
  //   return this.http.get<PersonSelectModel[]>(`${this.baseUrl}/${this.resource}/criteria`, { params: criteria });
  // }

  getByDocument(document: string): Observable<PersonSelectModel | null> {
    return this.http.get<PersonSelectModel | null>(`${environment.apiURL}/${this.resource}/document/${document}`);
  }

}
