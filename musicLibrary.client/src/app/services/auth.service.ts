import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private accessTokenKey = 'access_token';
  private refreshTokenKey = 'refresh_token';
  private apiUrl = 'http://localhost:5169/api/User';

  constructor(private http: HttpClient) {}

  // שמירת הטוקן
  setTokens(accessToken: string, refreshToken: string) {
    localStorage.setItem(this.accessTokenKey, accessToken);
    localStorage.setItem(this.refreshTokenKey, refreshToken);
  }

  // שליפת הטוקן
  getAccessToken(): string | null {
    return localStorage.getItem(this.accessTokenKey);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.refreshTokenKey);
  }

  // מחיקת הטוקן (ב־logout)
  clearTokens() {
    localStorage.removeItem(this.accessTokenKey);
    localStorage.removeItem(this.refreshTokenKey);
  }

  isLoggedIn(): boolean {
    return !!this.getAccessToken();
  }
  getUserName(): string | null {
    const token = this.getAccessToken();

    if (!token) return null;

    try {
      // decode the payload
      const decoded: any = jwtDecode(token);
      // ClaimTypes.Name becomes "unique_name" in JWT by default
    return decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || null;
    } catch {
      return null;
    }
  }
  async refreshTokens(): Promise<boolean> {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) return false;

    try {
      const res = await firstValueFrom(
        this.http.post<{ accessToken: string; refreshToken: string }>(`${this.apiUrl}/refresh`, {
          refreshToken,
        })
      );

      this.setTokens(res.accessToken, res.refreshToken);
      return true;
    } catch (err) {
      console.error('Token refresh failed', err);
      this.clearTokens();
      return false;
    }
  }
}
