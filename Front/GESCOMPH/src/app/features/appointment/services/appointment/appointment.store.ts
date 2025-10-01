import { Injectable } from "@angular/core";
import { BehaviorSubject, tap, catchError, throwError, Observable } from "rxjs";
import { AppointmentCreateModel, AppointmentSelectModel, AppointmentUpdateModel } from "../../models/appointment.models";
import { AppointmentService } from "./appointment.service";

@Injectable({
  providedIn: 'root'
})
export class AppointmentStore {
  private readonly _appointment = new BehaviorSubject<AppointmentSelectModel[]>([]);
  readonly appointments$ = this._appointment.asObservable();

  constructor(private appointmentService: AppointmentService) {
    this.loadAll();
  }

  private get appointment(): AppointmentSelectModel[] {
    return this._appointment.getValue();
  }

  private set appointment(val: AppointmentSelectModel[]) {
    this._appointment.next(val);
  }

  loadAll() {
    this.appointmentService.getAll().pipe(
      tap(data => this.appointment = data),
      catchError(err => {
        console.error('Error loading appointment', err);
        return throwError(() => err);
      })
    ).subscribe();
  }

  create(form: AppointmentCreateModel): Observable<AppointmentSelectModel> {
    return this.appointmentService.create(form).pipe(
      tap(() => {
        this.loadAll();
      })
    );
  }

  update(id: number, updateDto: AppointmentUpdateModel): Observable<AppointmentSelectModel> {
    return this.appointmentService.update(id, updateDto).pipe(
      tap(() => {
        this.loadAll();
      })
    );
  }

  delete(id: number): Observable<void> {
    return this.appointmentService.delete(id).pipe(
      tap(() => {
        this.appointment = this.appointment.filter(c => c.id !== id);
      })
    );
  }

  deleteLogic(id: number): Observable<void> {
    return this.appointmentService.deleteLogic(id).pipe(
      tap(() => {
        this.loadAll();
      })
    );
  }
  changeActiveStatus(id: number, active: boolean): Observable<AppointmentSelectModel> {
    return this.appointmentService.changeActiveStatus(id, active).pipe(
      tap(() => {
        this.loadAll();
      })
    );
  }
}
