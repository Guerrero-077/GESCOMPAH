import { Injectable, signal, computed } from '@angular/core';
import { User } from '../../../shared/models/user.model';

@Injectable({ providedIn: 'root' })
export class UserStore {
  private _user = signal<User | null>(null);

  // Señal pública reactiva
  readonly user = computed(() => this._user());

  set(user: User | null): void {
    this._user.set(user);
  }

  clear(): void {
    this._user.set(null);
  }

  get snapshot(): User | null {
    return this._user();
  }
}
