import { Injectable } from '@angular/core';
import { GenericService } from '../../../../core/service/generic/generic.service';
import { PersonSelectModel, PersonCreateModel, PersonUpdateModel } from '../../models/person.models';

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
}
