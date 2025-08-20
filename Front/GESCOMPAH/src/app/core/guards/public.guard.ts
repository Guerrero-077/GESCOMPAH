import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { PermissionService } from '../service/permission/permission.service';
import { map, of } from 'rxjs';

export const publicGuard: CanActivateFn = (route, state) => {
  const permissionService = inject(PermissionService);
  const router = inject(Router);

  // Si ya hay perfil en memoria, redirigimos al dashboard
  if (permissionService.currentUserProfile) {
    router.navigate(['/admin/dashboard'], { replaceUrl: true });
    return false;
  }

  // Si no hay perfil, permitimos el acceso a rutas públicas
  return true;
};
