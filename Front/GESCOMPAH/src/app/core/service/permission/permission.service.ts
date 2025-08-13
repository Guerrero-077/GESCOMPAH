import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { User } from '../../../shared/models/user.model';
import { USER_PROFILE_MOCK } from './user-profile.mock'; // Import the mock data

@Injectable({ providedIn: 'root' })
export class PermissionService {
  private userProfileSubject = new BehaviorSubject<User | null>(null);
  userProfile$: Observable<User | null> = this.userProfileSubject.asObservable();

  constructor() {
    // ⚠️ Solo para desarrollo local. Comentar en producción.
    // this.userProfileSubject.next(USER_PROFILE_MOCK as User);
  }

  public get currentUserProfile(): User | null {
    return this.userProfileSubject.value;
  }

  setUserProfile(profile: User | null): void {
    this.userProfileSubject.next(profile);
  }

  hasPermission(permission: string): boolean {
    const u = this.userProfileSubject.value;
    return !!u?.permissions?.includes(permission);
  }

  hasRole(role: string): boolean {
    const u = this.userProfileSubject.value;
    return !!u?.roles?.includes(role);
  }

  getMenu(): any[] | null {
    return this.userProfileSubject.value?.menu ?? null;
  }
}
