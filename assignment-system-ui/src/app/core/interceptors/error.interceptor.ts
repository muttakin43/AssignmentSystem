import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const authService = inject(AuthService);
  const snackBar = inject(MatSnackBar);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && !req.url.endsWith('/auth/login')) {
        authService.logout();
        router.navigate(['/login']);
      } else if (error.status === 403) {
        snackBar.open('You do not have permission to do that.', 'Dismiss', { duration: 4000 });
      } else if (error.status === 0 || error.status >= 500) {
        snackBar.open('Something went wrong. Please try again.', 'Dismiss', { duration: 4000 });
      }
      return throwError(() => error);
    }),
  );
};