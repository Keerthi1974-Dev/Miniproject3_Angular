import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, throwError } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { LoginDTO, RegisterDTO, AuthResponse } from '../models/user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private baseUrl = `${environment.apiUrl}/api/auth`;

  constructor(private http: HttpClient, private router: Router) {}

  register(dto: RegisterDTO): Observable<string> {
    return this.http.post(`${this.baseUrl}/register`, dto, { 
      responseType: 'text' 
    });
  }

  login(dto: LoginDTO): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/login`, dto, {
      withCredentials: true  // Needed for refresh cookie
    }).pipe(
      tap(response => {
        localStorage.setItem('token', response.token);
        localStorage.setItem('user', JSON.stringify(response.user));
      })
    );
  }

  // NEW - Auto refresh token
  refreshToken(): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/refresh`, {}, {
      withCredentials: true
    }).pipe(
      tap(response => {
        localStorage.setItem('token', response.token);
      }),
      catchError(error => {
        this.logout();
        return throwError(() => error);
      })
    );
  }

  //  Clear storage FIRST before HTTP call to prevent loop
  logout(): void {
    localStorage.removeItem('token');  //  moved up
    localStorage.removeItem('user');   // moved up
    this.http.post(`${this.baseUrl}/logout`, {}, {
      withCredentials: true
    }).subscribe({ error: () => {} }); // silently ignore errors
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getUser(): AuthResponse['user'] | null {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  isAdmin(): boolean {
    return this.getUser()?.role === 'Admin';
  }
}