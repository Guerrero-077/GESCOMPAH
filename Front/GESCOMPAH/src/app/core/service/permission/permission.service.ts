import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { User } from '../../../shared/models/user.model';
import { USER_PROFILE_MOCK } from './user-profile.mock'; // Import the mock data

@Injectable({
  providedIn: 'root'
})
export class PermissionService {
  private userProfileSubject = new BehaviorSubject<User | null>(null);
  userProfile$: Observable<User | null> = this.userProfileSubject.asObservable();

  constructor() {
    // For mocking purposes during development, uncomment the line below:
    // this.userProfileSubject.next(USER_PROFILE_MOCK as User);
  }

  // Getter to safely access the current value
  public get currentUserProfile(): User | null {
    return this.userProfileSubject.value;
  }

  setUserProfile(profile: User | null): void {
    this.userProfileSubject.next(profile);
  }

  hasPermission(permission: string): boolean {
    const userProfile = this.userProfileSubject.value;
    if (!userProfile || !userProfile.permissions) {
      return false;
    }
    return userProfile.permissions.includes(permission);
  }

  hasRole(role: string): boolean {
    const userProfile = this.userProfileSubject.value;
    if (!userProfile || !userProfile.roles) {
      return false;
    }
    return userProfile.roles.includes(role);
  }

  getMenu(): any[] | null { // Consider defining a more specific type for menu if possible
    const userProfile = this.userProfileSubject.value;
    return userProfile ? userProfile.menu : null;
  }
}
