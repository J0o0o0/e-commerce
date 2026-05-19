import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private http: HttpClient, private router: Router) { }

  // Login
  login(email: string, password: string) {
    return this.http.post<any>(`${environment.baseUrl}/api/auth/login`, { email, password });
  }

  // Register Buyer
  registerBuyer(body: any) {
    return this.http.post<any>(`${environment.baseUrl}/api/auth/register-buyer`, body);
  }

  // Register Seller
  registerSeller(body: any) {
    return this.http.post<any>(`${environment.baseUrl}/api/auth/register-seller`, body);
  }

  // Helper method to save token data
  saveUserData(res: any) {
    localStorage.setItem('token', res.token);
    localStorage.setItem('userName', res.userName);
    localStorage.setItem('email', res.email);
    localStorage.setItem('userId', res.userId);
    localStorage.setItem('role', res.role);
  }
}
