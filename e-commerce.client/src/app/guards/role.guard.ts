import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';

export const sellerGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const token = localStorage.getItem('token');
  const role = localStorage.getItem('role');

  // No token or no role → kick out
  if (!token || !role) {
    router.navigate(['/login']);
    return false;
  }

  // Check if token is expired
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    const isExpired = payload.exp * 1000 < Date.now();

    if (isExpired) {
      localStorage.clear();
      router.navigate(['/login']);
      return false;
    }
  } catch {
    localStorage.clear();
    router.navigate(['/login']);
    return false;
  }

  // Not a seller → kick to home
  if (role !== 'Seller') {
    router.navigate(['/']);
    return false;
  }

  return true;
};
