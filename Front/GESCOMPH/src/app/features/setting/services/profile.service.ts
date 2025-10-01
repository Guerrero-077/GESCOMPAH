import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { PersonSelectModel, PersonUpdateModel } from '../../security/models/person.models';
import { UserStore } from '../../../core/service/permission/User.Store';
import { PersonService } from '../../security/services/person/person.service';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private http = inject(HttpClient);
  private readonly baseUrl = environment.apiURL;
  private userStore = inject(UserStore);
  private personService = inject(PersonService);

  getProfile(): Observable<PersonSelectModel> {
    const user = this.userStore.user();
    if (user && user.personId) {
      return this.personService.getById(user.personId);
    } else {
      return throwError(() => new Error('User profile not found or personId is missing.'));
    }
  }

  /**
   * @deprecated The component should use PersonService directly.
   */
  updateProfile(id: number, dto: PersonUpdateModel): Observable<PersonSelectModel> {
    return this.personService.update(id, dto);
  }
}
