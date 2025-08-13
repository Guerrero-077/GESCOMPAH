import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { Person, PersonCreate, PersonUpdate } from '../../models/person.models';

@Injectable({
  providedIn: 'root'
})
export class PersonService {
  private readonly apiUrl = `${environment.apiURL}/person`;

  constructor(private http: HttpClient) { }

  getPersons(): Observable<Person[]> {
    return this.http.get<Person[]>(this.apiUrl);
  }

  createPerson(payload: PersonCreate): Observable<Person> {
    return this.http.post<Person>(this.apiUrl, payload);
  }

  updatePerson(id: number, payload: PersonUpdate): Observable<Person> {
    return this.http.put<Person>(`${this.apiUrl}/${id}`, payload);
  }

  deletePerson(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
