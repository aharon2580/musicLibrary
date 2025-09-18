import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
// imports: [CommonModule, HttpClientModule, MatTableModule, MatCardModule, MatToolbarModule]

// Angular Material
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatToolbarModule } from '@angular/material/toolbar';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatCardModule, MatToolbarModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  songs: any[] = [];
  playlists: any[] = [];

  displayedSongColumns: string[] = ['id', 'title', 'artist', 'album', 'durationSeconds'];
  displayedPlaylistColumns: string[] = ['id', 'name', 'userId', 'createdAt'];

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.http.get<any[]>('http://localhost:5169/api/Songs').subscribe((data) => {
      this.songs = data;
    });

    this.http.get<any[]>('http://localhost:5169/api/Playlists').subscribe((data) => {
      this.playlists = data;
    });
  }
}
