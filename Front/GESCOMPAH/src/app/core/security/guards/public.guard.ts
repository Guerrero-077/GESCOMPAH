import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { UserStore } from '../services/permission/User.Store';

export const publicGuard: CanActivateFn = (route, state) => {
  const userStore = inject(UserStore);
  const router = inject(Router);

  // Si ya hay usuario en memoria, redirige al dashboard
  if (userStore.snapshot) {
    router.navigate(['/dashboard'], { replaceUrl: true });
    return false;
  }

  // Si no hay usuario, puede acceder a ruta pública (ej: login, register)
  return true;
};

