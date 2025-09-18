import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-welcome',
  standalone: true,
  imports: [RouterLink, MatCardModule, MatButtonModule],
  template: `
    <div class="welcome-container">
      <mat-card>
        <mat-card-title>🎶 Welcome to Music Library</mat-card-title>
        <mat-card-content>
          <p>התחבר או הירשם כדי לגשת לספריית המוזיקה שלך</p>
          <div class="actions">
            <button mat-raised-button color="primary" routerLink="/login">Login</button>
            <button mat-raised-button color="accent" routerLink="/register">Register</button>
          </div>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .welcome-container {
      display: flex;
      justify-content: center;
      align-items: center;
      height: 100vh;
    }
    mat-card {
      padding: 20px;
      text-align: center;
    }
    .actions {
      margin-top: 20px;
      display: flex;
      justify-content: center;
      gap: 10px;
    }
  `]
})
export class WelcomeComponent {}
