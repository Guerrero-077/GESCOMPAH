import { Injectable } from '@angular/core';
import { BackendMenuItem } from '../../../shared/components/sidebar/sidebar.config';
import { UserStore } from './User.Store';

@Injectable({ providedIn: 'root' })
export class PermissionService {
  constructor(private userStore: UserStore) { }

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

  getMenu(): BackendMenuItem[] {
    return this.user?.menu ?? [];
  }
}
