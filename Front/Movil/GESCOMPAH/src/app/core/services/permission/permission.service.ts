import { Injectable, computed } from '@angular/core';
import { UserStore } from './User.Store';
import { User } from '../../../shared/models/user.model';
import { BackendMenuItem } from 'src/app/shared/models/sidebar.models';

@Injectable({ providedIn: 'root' })
export class PermissionService {
  constructor(private userStore: UserStore) { }

  readonly menu = computed(() => this.userStore.user()?.menu ?? []);

  // --- Stateless, pure functions for guards ---
  userHasRole(user: User, role: string): boolean {
    return user.roles?.includes(role) ?? false;
  }

  userHasPermissionForRoute(user: User, permission: string, url: string): boolean {
    const normalized = url.startsWith('/') ? url.slice(1) : url;
    return user.menu?.some(menu =>
      menu.forms?.some(form =>
        form.route === normalized && form.permissions?.includes(permission)
      )
    ) ?? false;
  }

  // --- Snapshot-based functions for other contexts ---
  private get user() {
    return this.userStore.snapshot;
  }

  hasPermission(permission: string): boolean {
    return this.user?.menu?.some(menu =>
      menu.forms?.some(form =>
        form.permissions?.includes(permission)
      )
    ) ?? false;
  }

  hasPermissionForRoute(permission: string, url: string): boolean {
    const normalized = url.startsWith('/') ? url.slice(1) : url;
    return this.user?.menu?.some(menu =>
      menu.forms?.some(form =>
        form.route === normalized && form.permissions?.includes(permission)
      )
    ) ?? false;
  }

  hasRole(role: string): boolean {
    return this.user?.roles?.includes(role) ?? false;
  }

  /**
   * @deprecated Use the `menu` signal instead for reactive contexts.
   */
  getMenu(): BackendMenuItem[] {
    return this.user?.menu ?? [];
  }
}
