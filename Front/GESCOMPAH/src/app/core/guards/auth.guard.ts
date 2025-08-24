import { inject } from '@angular/core';
import { CanActivateFn, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../service/auth/auth.service';
import { PermissionService } from '../service/permission/permission.service';
import { map, of, catchError } from 'rxjs';
import { UserStore } from '../service/permission/User.Store';

export const authGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
) => {
  const authService = inject(AuthService);
  const permissionService = inject(PermissionService);
  const userStore = inject(UserStore);
  const router = inject(Router);

  const check = () => {
    const profile = userStore.snapshot;

    if (!profile || !profile.roles) {
      router.navigate(['/auth/login']);
      return false;
    }

    if (!route || !route.data) {
      console.warn('AuthGuard: Route or route.data is missing. Allowing access by default, but this is unexpected.');
      return true;
    }

    // --- Role Check ---
    const requiredRoles = route.data['roles'] as string[] | undefined;
    if (requiredRoles?.length) {
      const userRoles = profile.roles;
      const hasRole = requiredRoles.some(requiredRole => userRoles.includes(requiredRole));
      if (!hasRole) {
        router.navigate(['/admin/dashboard']);
        return false;
      }
    }

    // --- Permission Check ---
    const requiredPermissions = route.data['permissions'] as string[] | undefined;
    if (requiredPermissions?.length) {
      const hasPermission = requiredPermissions.every(p =>
        permissionService.hasPermissionForRoute(p, state.url)
      );
      if (!hasPermission) {
        router.navigate(['/admin/dashboard']);
        return false;
      }
    }

    return true;
  };

  if (userStore.snapshot) {
    return check();
  }

  return authService.GetMe().pipe(
    map(user => {
      if (user) {
        userStore.set(user);
        return check();
      }
      router.navigate(['/auth/login']);
      return false;
    }),
    catchError(() => {
      router.navigate(['/auth/login']);
      return of(false);
    })
  );
};
