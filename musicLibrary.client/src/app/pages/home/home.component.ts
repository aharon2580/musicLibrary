import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { AuthService } from '../../services/auth.service';
import { PlaylistService } from '../../services/playlist.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatToolbarModule, MatButtonModule, MatTableModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  songs: any[] = [];
  playlists: any[] = [];
  displayedPlaylistColumns: string[] = ['name', 'createdAt'];

  userName: string | null = null;

  // popup states
  showPlayPopup = false;
  showPlaylistPopup = false;
  selectedSong: any = null;

  constructor(
    private http: HttpClient,
    private authService: AuthService,
    private playlistService: PlaylistService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.userName = this.authService.getUserName();

    this.http.get<any[]>('http://localhost:5169/api/Songs').subscribe((data) => {
      this.songs = data;
    });

    this.http.get<any[]>('http://localhost:5169/api/Playlists').subscribe((data) => {
      this.playlists = data;
    });
  }

  logout(): void {
    this.authService.clearTokens();
    this.router.navigate(['']);
  }

  // === popup methods ===
  openPlayPopup(song: any): void {
    this.selectedSong = song;
    this.showPlayPopup = true;
  }

  closePlayPopup(): void {
    this.showPlayPopup = false;
    this.selectedSong = null;
  }

  openAddToPlaylistPopup(song: any): void {
    this.selectedSong = song;
    this.showPlaylistPopup = true;
  }

  closePlaylistPopup(): void {
    this.showPlaylistPopup = false;
    this.selectedSong = null;
  }

  async selectPlaylist(playlist: any): Promise <void> {
    await this.playlistService.addSongToPlaylist(playlist.id, this.selectedSong?.id, 0);
    alert(`✅ השיר "${this.selectedSong?.title}" נוסף ל"${playlist.name}" בהצלחה!`);
    console.log(`Selected playlist: ${playlist.name} for song ${this.selectedSong?.title}`);
    // בעתיד נבצע כאן קריאת POST להוספת השיר
    this.closePlaylistPopup();
  }

  createNewPlaylist(): void {
    console.log(`Create new playlist for song ${this.selectedSong?.title}`);
    // בהמשך נפתח כאן דיאלוג יצירת פלייליסט חדש
    this.closePlaylistPopup();
  }
}
