import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { PermissionService } from '../service/permission/permission.service';

export const authGuard: CanActivateFn = (route, state) => {
  const permissionService = inject(PermissionService);
  const router = inject(Router);

  if (permissionService.currentUserProfile) {
    return true; // Si hay un perfil de usuario, permite el acceso
  }

  // Si no hay perfil, redirige a la página de login, reemplazando la URL en el historial
  router.navigate(['/Auth/login'], { replaceUrl: true });
  return false;
};
