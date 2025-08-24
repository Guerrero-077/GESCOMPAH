import { inject } from '@angular/core';
import { CanActivateFn, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../service/auth/auth.service';
import { PermissionService } from '../service/permission/permission.service';
import { map, of, catchError } from 'rxjs';
import { UserStore } from '../service/permission/User.Store';
import { User } from '../../shared/models/user.model';

export const authGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
) => {
  const authService = inject(AuthService);
  const permissionService = inject(PermissionService);
  const userStore = inject(UserStore);
  const router = inject(Router);

  const check = (profile: User) => {
    if (!profile.roles) {
      router.navigate(['/auth/login']);
      return false;
    }

    if (!route.data) {
      return true; // No requirements, allow access
    }

    // --- Role Check ---
    const requiredRoles = route.data['roles'] as string[] | undefined;
    if (requiredRoles?.length) {
      const hasRole = requiredRoles.some(role => permissionService.userHasRole(profile, role));
      if (!hasRole) {
        router.navigate(['/admin/dashboard']); // Or a dedicated 'unauthorized' page
        return false;
      }
    }

    // --- Permission Check ---
    const requiredPermissions = route.data['permissions'] as string[] | undefined;
    if (requiredPermissions?.length) {
      const hasPermission = requiredPermissions.every(p =>
        permissionService.userHasPermissionForRoute(profile, p, state.url)
      );
      if (!hasPermission) {
        router.navigate(['/admin/dashboard']); // Or a dedicated 'unauthorized' page
        return false;
      }
    }

    return true;
  };

  const currentUser = userStore.snapshot;
  if (currentUser) {
    return check(currentUser);
  }

  return authService.GetMe().pipe(
    map(user => {
      if (user) {
        userStore.set(user);
        return check(user);
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
