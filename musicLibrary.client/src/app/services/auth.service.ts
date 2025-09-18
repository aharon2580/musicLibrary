import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private tokenKey = 'accessToken';

  // שמירת הטוקן אחרי login
  setToken(accessToken: string): void {
    localStorage.setItem(this.tokenKey, accessToken);
    console.log('Token set:', accessToken);
  }

  // שליפת הטוקן
  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  // מחיקת הטוקן (ב־logout)
  clearToken(): void {
    localStorage.removeItem(this.tokenKey);
  }

  // בדיקה אם המשתמש מחובר
  isAuthenticated(): boolean {
    const token = this.getToken();
    return !!token; // אפשר לשדרג לבדיקה אם הטוקן פג תוקף
  }
}
