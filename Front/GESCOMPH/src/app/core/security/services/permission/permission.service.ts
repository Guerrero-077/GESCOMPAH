import { Injectable, computed } from '@angular/core';
import { BackendMenuItem } from '../../../../shared/components/sidebar/sidebar.config';
import { User } from '../../../../shared/models/user.model';
import { normalizeUrl } from '../../../utils/url-normalize';
import { UserStore } from './User.Store';

@Injectable({ providedIn: 'root' })
export class PermissionService {
  constructor(private readonly userStore: UserStore) {}

  // === Derivados reactivos ===================================================

  // Menú reactivo (para componentes)
  readonly menu = computed<BackendMenuItem[]>(() => this.userStore.user()?.menu ?? []);

  // Índices reactivos para consultas en O(1)
  // 1) ruta -> Set(permisos)
  readonly routeToPerms = computed(() => {
    const map = new Map<string, Set<string>>();
    for (const mod of this.userStore.user()?.menu ?? []) {
      for (const form of mod.forms ?? []) {
        map.set(form.route, new Set(form.permissions ?? []));
      }
    }
    return map;
  });

  // 2) roles -> Set
  readonly roleSet = computed(() => new Set(this.userStore.user()?.roles ?? []));

  // === Funciones "puras" para guards (reciben User explícito) ================
  userHasRole(user: User, role: string): boolean {
    return (user.roles ?? []).includes(role);
  }

  userHasPermissionForRoute(user: User, permission: string, url: string): boolean {
    const route = normalizeUrl(url);
    const index = new Map<string, Set<string>>();
    for (const m of user.menu ?? [])
      for (const f of m.forms ?? [])
        index.set(f.route, new Set(f.permissions ?? []));
    return index.get(route)?.has(permission) ?? false;
  }

  // === Conveniencia por snapshot (no reactivas) ==============================
  private get user(): User | null {
    return this.userStore.snapshot;
  }

  hasRole(role: string): boolean {
    return this.roleSet().has(role);
  }

  hasPermission(permission: string): boolean {
    // Busca permiso en cualquier ruta
    for (const perms of this.routeToPerms().values()) {
      if (perms.has(permission)) return true;
    }
    return false;
  }

  hasPermissionForRoute(permission: string, url: string): boolean {
    const route = normalizeUrl(url);
    return this.routeToPerms().get(route)?.has(permission) ?? false;
  }

  /**
   * @deprecated Usa la señal `menu` para contextos reactivos.
   */
  getMenu(): BackendMenuItem[] {
    return this.user?.menu ?? [];
  }
}
