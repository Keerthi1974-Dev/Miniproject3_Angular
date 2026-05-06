import { Injectable } from '@angular/core';
import {
  HttpInterceptor, HttpRequest,
  HttpHandler, HttpEvent, HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { AuthService } from '../services/auth';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private authService: AuthService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    
    //  Attach token to every request automatically
    const token = this.authService.getToken();
    const authReq = token ? req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    }) : req;

    return next.handle(authReq).pipe(
      catchError((error: HttpErrorResponse) => {
        
        // ✅ FIX 1: Skip refresh logic for auth calls to prevent infinite loop
        const isAuthCall = req.url.includes('/api/auth');

        // Token expired → silently refresh
        if (error.status === 401 && !isAuthCall) {
          return this.authService.refreshToken().pipe(
            switchMap(response => {
              // Retry original request with new token
              const retryReq = authReq.clone({
                setHeaders: {
                  Authorization: `Bearer ${response.token}`
                }
              });
              return next.handle(retryReq);
            }),
            catchError(() => {
              // Refresh failed → go to login
              this.authService.logout();
              return throwError(() => error);
            })
          );
        }
        return throwError(() => error);
      })
    );
  }
}