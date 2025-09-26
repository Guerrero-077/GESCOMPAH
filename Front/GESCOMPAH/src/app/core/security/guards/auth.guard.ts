import { inject } from '@angular/core';
import { CanActivateFn, Router, UrlTree, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { of, map, catchError } from 'rxjs';
import { AuthService } from '../services/auth/auth.service';
import { User } from '../../../shared/models/user.model';
import { UserStore } from '../services/permission/User.Store';
import { normalizeUrl } from '../../utils/url-normalize';


export const authGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
) => {
  const router = inject(Router);
  const auth = inject(AuthService);
  const userStore = inject(UserStore);

  const buildAllowed = (u: User) => new Set(
    (u.menu ?? []).flatMap(m => (m.forms ?? []).map(f => f.route))
  );

  const fallbackUrl = (u: User | null): UrlTree => {
    const hasDashboard = !!(u?.menu ?? []).some(m => (m.forms ?? []).some(f => f.route === 'dashboard'));
    return router.parseUrl(hasDashboard ? '/dashboard' : '/auth/login');
  };

  const can = (u: User | null): boolean | UrlTree => {
    if (!u?.menu?.length) return router.parseUrl('/auth/login');

    const allowed = buildAllowed(u);
    const req = normalizeUrl(state.url);

    // Si ruta vacía, permite si existe 'dashboard'
    if (req === '' && allowed.has('dashboard')) return true;

    // Chequeo progresivo de segmentos: admin/users/edit -> admin -> admin/users -> admin/users/edit
    let curr = '';
    for (const seg of req.split('/').filter(Boolean)) {
      curr = curr ? `${curr}/${seg}` : seg;
      if (allowed.has(curr)) return true;
    }

    return fallbackUrl(u);
  };

  // Siempre valida sesión contra backend antes de activar la ruta
  return auth.GetMe().pipe(
    map(u => {
      if (!u) return router.parseUrl('/auth/login');
      userStore.set(u);
      return can(u);
    }),
    catchError(() => of(router.parseUrl('/auth/login')))
  );
};
