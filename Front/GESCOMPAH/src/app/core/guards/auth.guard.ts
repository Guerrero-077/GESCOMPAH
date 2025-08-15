import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { PermissionService } from '../service/permission/permission.service';
import { AuthService } from '../service/auth/auth.service';
import { catchError, map, of } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const permissionService = inject(PermissionService);
  const authService = inject(AuthService);
  const router = inject(Router);

  // Si ya tenemos el perfil en memoria, permitimos el acceso
  if (permissionService.currentUserProfile) {
    return true;
  }

  // Si no hay perfil, intentamos obtenerlo
  return authService.GetMe().pipe(
    map(() => true),
    catchError(() => {
      router.navigate(['/auth/login'], { replaceUrl: true });
      return of(false);
    })
  );
};
