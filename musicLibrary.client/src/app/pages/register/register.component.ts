import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent {
  registerData = {
    userName: '',
    email: '',
    password: '',
    confirmPassword: '',
  };

  registerError: string | null = null;
  registerSuccess: string | null = null;
  loading = false;

  constructor(private http: HttpClient, private authService: AuthService, private router: Router) {}

  register() {
    // reset state
    this.registerError = null;
    this.registerSuccess = null;

    // basic validation
    if (!this.registerData.userName || !this.registerData.email || !this.registerData.password) {
      this.registerError = 'All fields are required';
      return;
    }
    if (this.registerData.password !== this.registerData.confirmPassword) {
      this.registerError = 'Passwords do not match';
      return;
    }
    this.loading = true;

    const { confirmPassword, ...payload } = this.registerData;

    this.http
      .post<{ accessToken: string; refreshToken: string }>(
        'http://localhost:5169/api/User/register',
        payload 
      )
      .subscribe({
        next: (res) => {
          console.log('Registration successful:', res);
          this.authService.setTokens(res.accessToken, res.refreshToken);
          this.registerSuccess = 'Registration successful!';
          this.router.navigate(['/home']);
        },
        error: (err) => {
          console.error('Registration failed:', err);
          this.registerError = err.error?.message || 'Registration failed. Try again.';
        },
        complete: () => {
          this.loading = false;
          if (this.registerSuccess) {
            this.registerData = { userName: '', email: '', password: '', confirmPassword: '' };
          }
        },
      });
  }
}
