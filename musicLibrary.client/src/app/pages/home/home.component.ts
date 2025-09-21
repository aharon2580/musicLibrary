import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

// Angular Material
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';

import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatCardModule, MatToolbarModule, MatButtonModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  songs: any[] = [];
  playlists: any[] = [];

  displayedSongColumns: string[] = ['id', 'title', 'artist', 'album', 'durationSeconds'];
  displayedPlaylistColumns: string[] = ['id', 'name', 'userId', 'createdAt'];

  constructor(
    private http: HttpClient,
    private authService: AuthService,
    private router: Router
  ) {}

  userName: string | null = null;
  role: string | null = null;
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
}
