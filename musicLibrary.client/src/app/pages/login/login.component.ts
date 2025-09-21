import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatInputModule,
    MatFormFieldModule,
    MatButtonModule,
    MatCardModule,
  ],
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
    this.http
      .post<{ accessToken: string; refreshToken: string }>(
        'http://localhost:5169/api/User/login',
        this.loginData
      )
      .subscribe({
        next: (res) => {
          this.authService.setTokens(res.accessToken, res.refreshToken);
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
