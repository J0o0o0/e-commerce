import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {

  firstName = '';
  lastName = '';
  userName = '';
  email = '';
  password = '';
  confirmPassword = '';
  phoneNumber = '';
  shippingAddress = '';
  errorMessages: String[] = [];

  constructor(
    private http: HttpClient,
    private authService: AuthService,
    private router: Router
  ) { }

  register(): void {
    this.errorMessages = [];

    // Basic validation
    if (!this.firstName || !this.lastName || !this.userName ||
      !this.email || !this.password || !this.confirmPassword ||
      !this.phoneNumber || !this.shippingAddress) {
       this.errorMessages = ['All fields are required.'];
      return;
    }

    const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    if (!emailPattern.test(this.email)) {
      this.errorMessages = ['Please enter a valid email address.'];
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.errorMessages = ['Passwords do not match.'];
      return;
    }

    const phonePattern = /^[0-9]{11}$/;
    if (!phonePattern.test(this.phoneNumber)) {
      this.errorMessages = ['Phone number must be exactly 11 digits (numbers only).'];
      return;
    }

    const body = {
      userName: this.userName,
      email: this.email,
      password: this.password,
      firstName: this.firstName,
      lastName: this.lastName,
      shippingAddress: this.shippingAddress,
      phoneNumber: this.phoneNumber
    };

    this.authService.registerBuyer(body).subscribe({
      next: () => {
        // Auto login after registration
        this.authService.login(this.email, this.password).subscribe({
          next: (res) => {
            this.authService.saveUserData(res);
            this.router.navigate(['/']);
          },
          error: () => {
            // If auto login fails, redirect to login page
            this.router.navigate(['/login']);
          }
        });
      },
      error: (err: any) => {
        const apiErrors = err?.error;

        if (typeof apiErrors === 'string') {
          // API returned plain text — split by comma+space
          this.errorMessages = apiErrors.split(', ');
        } else if (Array.isArray(apiErrors) && apiErrors.length > 0) {
          this.errorMessages = apiErrors;
        } else if (apiErrors?.message) {
          this.errorMessages = [apiErrors.message];
        } else if (apiErrors?.errors) {
          const messages = Object.values(apiErrors.errors).flat();
          this.errorMessages = messages as string[];
        } else {
          this.errorMessages = ['Registration failed. Please try again.'];
        }
      }
    });
  }
}
