import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { PersonCreateModel, PersonSelectModel, PersonUpdateModel } from '../../models/person.models';
import { PersonService } from './person.service';

@Injectable({
  providedIn: 'root'
})
export class PersonStore {
  private readonly _persons = new BehaviorSubject<PersonSelectModel[]>([]);
  readonly persons$ = this._persons.asObservable();

  constructor(private personService: PersonService) {
    this.loadAll();
  }

  private get persons(): PersonSelectModel[] {
    return this._persons.getValue();
  }

  private set persons(val: PersonSelectModel[]) {
    this._persons.next(val);
  }

  loadAll() {
    this.personService.getAll().pipe(
      tap(data => this.persons = data),
      catchError(err => {
        console.error('Error loading persons', err);
        return throwError(() => err);
      })
    ).subscribe();
  }

  create(person: PersonCreateModel): Observable<PersonSelectModel> {
    return this.personService.create(person).pipe(
      tap(() => {
        this.loadAll(); // Force refresh
      })
    );
  }

  update(id: number, updateDto: PersonUpdateModel): Observable<PersonSelectModel> {
    return this.personService.update(id, updateDto).pipe(
      tap(() => {
        this.loadAll(); // Force refresh
      })
    );
  }

  delete(id: number): Observable<void> {
    return this.personService.delete(id).pipe(
      tap(() => {
        this.persons = this.persons.filter(c => c.id !== id);
      })
    );
  }

  deleteLogic(id: number): Observable<void> {
    return this.personService.deleteLogic(id).pipe(
      tap(() => {
        this.loadAll(); // Force refresh
      })
    );
  }


  changeActiveStatus(id: number, active: boolean): Observable<PersonSelectModel> {
    return this.personService.changeActiveStatus(id, active).pipe(
      tap(() => {
        this.loadAll()
      })
    );
  }
}
