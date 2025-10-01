import { Injectable } from '@angular/core';
import { Subject, Observable } from 'rxjs';

export type AuthEvent =
  | { type: 'SESSION_EXPIRED' }
  | { type: 'UNAUTHORIZED' }
  | { type: 'LOGOUT' };

@Injectable({ providedIn: 'root' })
export class AuthEventsService {
  private authEvents$ = new Subject<AuthEvent>();

  emit(event: AuthEvent): void {
    this.authEvents$.next(event);
  }

  onEvents(): Observable<AuthEvent> {
    return this.authEvents$.asObservable();
  }

  // Helpers para emitir eventos específicos
  sessionExpired() {
    this.emit({ type: 'SESSION_EXPIRED' });
  }

  unauthorized() {
    this.emit({ type: 'UNAUTHORIZED' });
  }

  logout() {
    this.emit({ type: 'LOGOUT' });
  }
}
