import { inject } from '@angular/core';
import { CanActivateFn, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { PermissionService } from '../service/permission/permission.service';
import { AuthService } from '../service/auth/auth.service';
import { map, of, catchError } from 'rxjs';

export const authGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
) => {
  const permissionService = inject(PermissionService);
  const authService = inject(AuthService);
  const router = inject(Router);

  const check = () => {
    const profile = permissionService.currentUserProfile;

    if (!profile || !profile.roles) {
      router.navigate(['/auth/login']);
      return false;
    }

    // --- Defensive check for route.data ---
    if (!route || !route.data) {
      console.warn('AuthGuard: Route or route.data is missing. Allowing access by default, but this is unexpected.');
      return true;
    }

    // --- Role Check ---
    const requiredRoles = route.data['roles'] as string[] | undefined;
    if (requiredRoles && requiredRoles.length > 0) {
      const userRoles = profile.roles;
      const hasRole = requiredRoles.some(requiredRole => userRoles.includes(requiredRole));
      if (!hasRole) {
        router.navigate(['/admin/dashboard']);
        return false;
      }
    }

    // --- Permission Check ---
    const requiredPermissions = route.data['permissions'] as string[] | undefined;
    if (requiredPermissions && requiredPermissions.length > 0) {
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

  if (permissionService.currentUserProfile) {
    return check();
  }

  return authService.GetMe().pipe(
    map(user => {
      if (user) {
        permissionService.setUserProfile(user);
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

