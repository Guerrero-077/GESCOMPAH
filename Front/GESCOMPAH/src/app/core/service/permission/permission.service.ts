import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { User } from '../../../shared/models/user.model';
import { BackendMenuItem } from '../../../shared/components/sidebar/sidebar.config';

@Injectable({ providedIn: 'root' })
export class PermissionService {
  private userProfileSubject = new BehaviorSubject<User | null>(null);
  readonly userProfile$: Observable<User | null> = this.userProfileSubject.asObservable();

  constructor() {
    // The user profile should be set by an authentication service upon login.
  }

  get currentUserProfile(): User | null {
    return this.userProfileSubject.value;
  }

  setUserProfile(profile: User | null): void {
    this.userProfileSubject.next(profile);
  }

  clearUserProfile(): void {
    this.userProfileSubject.next(null);
  }

  /**
   * Checks if the user has a global permission.
   * @param permission The permission string to check.
   * @returns True if the user has the permission, false otherwise.
   */
  hasPermission(permission: string): boolean {
    const user = this.currentUserProfile;
    if (!user || !user.menu) {
      return false;
    }

    for (const menuItem of user.menu) {
      if (menuItem.forms) {
        for (const form of menuItem.forms) {
          if (form.permissions?.includes(permission)) {
            return true; // Permission found, return true immediately
          }
        }
      }
    }

    return false; // Permission not found in any form
  }

  /**
   * Checks if the user has a specific permission for a given route.
   * It looks into the user's menu structure.
   * @param permission The permission string to check.
   * @param url The route URL to check against.
   * @returns True if the user has the permission for that route, false otherwise.
   */
  hasPermissionForRoute(permission: string, url: string): boolean {
    const user = this.currentUserProfile;
    if (!user || !user.menu) {
      return false;
    }

    const normalizedUrl = url.startsWith('/') ? url.substring(1) : url;

    for (const menuItem of user.menu) {
      if (menuItem.forms) {
        for (const form of menuItem.forms) {
          if (form.route === normalizedUrl) {
            return form.permissions?.includes(permission) ?? false;
          }
        }
      }
    }
    return false;
  }

  hasRole(role: string): boolean {
    return this.currentUserProfile?.roles?.includes(role) ?? false;
  }

  getMenu(): BackendMenuItem[] | null {
    return this.currentUserProfile?.menu ?? null;
  }
}
