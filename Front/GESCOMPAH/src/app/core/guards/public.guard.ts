import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { PermissionService } from '../service/permission/permission.service';

export const publicGuard: CanActivateFn = (route, state) => {
  const permissionService = inject(PermissionService);
  const router = inject(Router);

  if (permissionService.currentUserProfile) {
    // Si el usuario ya está logueado, redirige al dashboard, reemplazando la URL en el historial
    router.navigate(['/admin/dashboard'], { replaceUrl: true });
    return false;
  }

  // Si no hay usuario, permite el acceso a la página de login/registro
  return true;
};
