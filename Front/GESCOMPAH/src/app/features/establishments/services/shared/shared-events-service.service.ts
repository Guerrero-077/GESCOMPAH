import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SharedEventsServiceService {
  private _plazaStateChanged = new Subject<number>(); // puede emitir ID o null
  plazaStateChanged$ = this._plazaStateChanged.asObservable();

  notifyPlazaStateChanged(plazaId: number) {
    this._plazaStateChanged.next(plazaId);
  }
}
