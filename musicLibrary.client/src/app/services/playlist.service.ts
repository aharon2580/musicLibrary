import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class PlaylistService {
  private apiUrl = 'http://localhost:5169/api/Playlists';

  constructor(private http: HttpClient, private auth: AuthService) {}

  private getAuthHeaders(): HttpHeaders {
    const token = this.auth.getAccessToken();
    return new HttpHeaders({
      Authorization: token ? `Bearer ${token}` : '',
      'Content-Type': 'application/json',
    });
  }

  /** 📜 שליפת כל הפלייליסטים של המשתמש */
  async getPlaylists(): Promise<any[]> {
    return await firstValueFrom(
      this.http.get<any[]>(this.apiUrl, { headers: this.getAuthHeaders() })
    );
  }

  /** 🆕 יצירת פלייליסט חדש */
  async createPlaylist(name: string): Promise<any> {
    return await firstValueFrom(
      this.http.post<any>(
        this.apiUrl,
        { name },
        { headers: this.getAuthHeaders() }
      )
    );
  }

  /** 🎵 הוספת שיר לפלייליסט קיים */
  async addSongToPlaylist(playlistId: number, songId: number, order: number = 0): Promise<any> {
    const url = `${this.apiUrl}/${playlistId}/add-song`;
    const body = { songId, order };
    return await firstValueFrom(
      this.http.post<any>(url, body, { headers: this.getAuthHeaders() })
    );
  }
}
