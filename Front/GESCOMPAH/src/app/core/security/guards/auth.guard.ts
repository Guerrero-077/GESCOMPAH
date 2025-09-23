import { inject } from '@angular/core';
import { CanActivateFn, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../../service/auth/auth.service';
import { map, of, catchError } from 'rxjs';
import { UserStore } from '../../service/permission/User.Store';
import { User } from '../../../shared/models/user.model';

export const authGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
) => {
  const authService = inject(AuthService);
  const userStore = inject(UserStore);
  const router = inject(Router);

  const check = (profile: User) => {
    if (!profile || !profile.menu) {
      router.navigate(['/auth/login']);
      return false;
    }

    const allowedRoutes = profile.menu.flatMap(module => module.forms.map(form => form.route));

    const requestedRoute = state.url.split('?')[0].substring(1); // remove leading '/'

    if (requestedRoute === '') {
      if (allowedRoutes.includes('dashboard')) {
        return true;
      }
    }

    const urlSegments = requestedRoute.split('/');
    let currentRouteToCheck = '';
    for (let i = 0; i < urlSegments.length; i++) {
      currentRouteToCheck += (i > 0 ? '/' : '') + urlSegments[i];
      if (allowedRoutes.includes(currentRouteToCheck)) {
        return true;
      }
    }

    if (allowedRoutes.includes('dashboard')) {
      router.navigate(['/dashboard']);
    } else {
      router.navigate(['/auth/login']);
    }
    return false;
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
