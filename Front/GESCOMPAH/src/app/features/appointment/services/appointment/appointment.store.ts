import { Injectable } from "@angular/core";
import { BehaviorSubject, tap, catchError, throwError, Observable } from "rxjs";
import { AppointmentCreateModel, AppointmentSelect, AppointmentUpdateModel } from "../../models/appointment.models";
import { AppointmentService } from "./appointment.service";

@Injectable({
  providedIn: 'root'
})
export class AppointmentStore {
  private readonly _appointment = new BehaviorSubject<AppointmentSelect[]>([]);
  readonly appointments$ = this._appointment.asObservable();

  constructor(private appointmentService: AppointmentService) {
    this.loadAll();
  }

  private get appointment(): AppointmentSelect[] {
    return this._appointment.getValue();
  }

  private set appointment(val: AppointmentSelect[]) {
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

  loadById(id : number): AppointmentSelect | undefined {
    return this.appointment.find(c => c.id === id);
  }


  create(form: AppointmentCreateModel): Observable<AppointmentSelect> {
    return this.appointmentService.create(form).pipe(
      tap(() => {
        this.loadAll();
      })
    );
  }

  update(id: number, updateDto: AppointmentUpdateModel): Observable<AppointmentSelect> {
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
  changeActiveStatus(id: number, active: boolean): Observable<AppointmentSelect> {
    return this.appointmentService.changeActiveStatus(id, active).pipe(
      tap(() => {
        this.loadAll();
      })
    );
  }
}
