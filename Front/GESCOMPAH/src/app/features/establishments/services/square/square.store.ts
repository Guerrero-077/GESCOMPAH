import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable, throwError } from "rxjs";
import { SquareCreateModel, SquareSelectModel, SquareUpdateModel } from "../../models/squares.models";
import { SquareService } from './square.service';
import { catchError, tap } from 'rxjs/operators';
import { ReturnStatement } from "@angular/compiler";

@Injectable({
  providedIn: 'root'
})

export class SquareStore {
  private readonly _squares = new BehaviorSubject<SquareSelectModel[]>([]);
  readonly squares$ = this._squares.asObservable();

  constructor(private squareService: SquareService) {
    this.loadAll()
  }

  private get squares(): SquareSelectModel[] {
    return this._squares.getValue();
  }

  private set squares(val: SquareSelectModel[]) {
    this._squares.next(val);
  }

  loadAll() {
    this.squareService.getAll().pipe(
      tap(data => this.squares = data),
      catchError(err => {
        console.error('Error loading squares', err);
        return throwError(() => err);
      })

    ).subscribe();
  }

  create(square: SquareCreateModel): Observable<SquareSelectModel> {
    return this.squareService.create(square).pipe(
      tap(() => {
        this.loadAll();

      })
    )
  }

  update(id: number, updateDto: SquareUpdateModel): Observable<SquareSelectModel> {
    return this.squareService.update(id, updateDto).pipe(
      tap(() => {
        this.loadAll();
      })
    )
  }

  delete(id: number): Observable<void> {
    return this.squareService.delete(id).pipe(
      tap(() => {
        this.squares = this.squares.filter(c => c.id !== id);
      })
    );
  }

  deleteLogical(id: number): Observable<void> {
    return this.squareService.deleteLogic(id).pipe(
      tap(() => {
        this.loadAll();
      })
    )
  }

  changeActiveStatus(id: number, active: boolean): Observable<SquareSelectModel> {
    return this.squareService.changeActiveStatus(id, active).pipe(
      tap(() => {
        this.loadAll()
      })
    );
  }
}
