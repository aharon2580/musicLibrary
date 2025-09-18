import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule], // נדרש בשביל ngModel ו-*ngIf
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent {
  // מודל לטופס
  loginData = {
    email: '',
    password: '',
  };

  // הודעת שגיאה אם נכשל login
  loginError: string | null = null;

  // אינדיקציה לטעינה (למשל spinner)
  loading = false;

  constructor(private http: HttpClient, private authService: AuthService, private router: Router) {}

  login() {
    this.loading = true;
    this.loginError = null;
    console.log('Attempting login with', this.loginData);
    this.http
      .post<{ accessToken: string }>('http://localhost:5169/api/User/login', this.loginData)
      .subscribe({
        next: (res) => {
          this.authService.setToken(res.accessToken);
          console.log('Login successful with :', res);
          this.router.navigate(['/home']);
        },
        error: (err) => {
          console.error('Login failed:', err);
          this.loginError = 'Invalid username or password';
        },
        complete: () => {
          this.loading = false;
        },
      });
  }
}
