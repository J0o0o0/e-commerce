import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const token = localStorage.getItem('token');
  const role = localStorage.getItem('role');

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

  const expectedRole = route.data['role'];

  if (expectedRole && role !== expectedRole) {
    router.navigate([role === 'Seller' ? '/seller/dashboard' : '/']);
    return false;
  }

  return true;
};
