import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { UserCreateModel, UserSelectModel, UserUpdateModel } from '../../models/user.models';
import { UserService } from './user.service';

@Injectable({
  providedIn: 'root'
})
export class UserStore {
  private readonly _users = new BehaviorSubject<UserSelectModel[]>([]);
  readonly users$ = this._users.asObservable();

  constructor(private userService: UserService) {
    this.loadAll();
  }

  private get users(): UserSelectModel[] {
    return this._users.getValue();
  }

  private set users(val: UserSelectModel[]) {
    this._users.next(val);
  }

  loadAll() {
    this.userService.getAll().pipe(
      tap(data => this.users = data),
      catchError(err => {
        console.error('Error loading users', err);
        return throwError(() => err);
      })
    ).subscribe();
  }

  create(user: UserCreateModel): Observable<UserSelectModel> {
    return this.userService.create(user).pipe(
      tap(() => {
        this.loadAll(); // Force refresh
      })
    );
  }

  update(id: number, updateDto: UserUpdateModel): Observable<UserSelectModel> {
    return this.userService.update(id, updateDto).pipe(
      tap(() => {
        this.loadAll(); // Force refresh
      })
    );
  }

  delete(id: number): Observable<void> {
    return this.userService.delete(id).pipe(
      tap(() => {
        this.users = this.users.filter(c => c.id !== id);
      })
    );
  }

  deleteLogic(id: number): Observable<void> {
    return this.userService.deleteLogic(id).pipe(
      tap(() => {
        this.loadAll(); // Force refresh
      })
    );
  }

  changeActiveStatus(id: number, active: boolean): Observable<UserSelectModel> {
    return this.userService.changeActiveStatus(id, active).pipe(
      tap(() => {
        this.loadAll(); // Force refresh
      })
    );
  }
}
