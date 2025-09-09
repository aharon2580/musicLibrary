using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using OneProject.Server.Data;
using OneProject.Server.Generated;
using OneProject.Server.Models.DTOs;

namespace OneProject.Server.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PlaylistService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Playlist>> GetAllAsync()
        {
            var userId = GetCurrentUserId();
            var isAdmin = _httpContextAccessor.HttpContext?.User?.IsInRole("Admin") == true;

            var query = _context.Playlists.AsNoTracking().AsQueryable();
            if (!isAdmin)
            {
                query = query.Where(p => p.UserId == userId);
            }

            var lists = await query.ToListAsync();
            foreach (var p in lists)
            {
                p.Items = await _context.PlaylistSongs.AsNoTracking()
                    .Where(ps => ps.PlaylistId == p.Id)
                    .OrderBy(ps => ps.Order)
                    .Select(ps => new PlaylistItem
                    {
                        SongId = ps.SongId,
                        Order = ps.Order
                    }).ToListAsync();
            }
            return lists;
        }

        public async Task<Playlist?> GetByIdAsync(int id)
        {
            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist == null) return null;
            var userId = GetCurrentUserId();
            var isAdmin = _httpContextAccessor.HttpContext?.User?.IsInRole("Admin") == true;
            if (!isAdmin && playlist.UserId != userId) return null;

            playlist.Items = await _context.PlaylistSongs.AsNoTracking()
                .Where(ps => ps.PlaylistId == playlist.Id)
                .OrderBy(ps => ps.Order)
                .Select(ps => new PlaylistItem { SongId = ps.SongId, Order = ps.Order })
                .ToListAsync();
            return playlist;
        }

        public async Task<Playlist> CreateAsync(CreatePlaylist dto)
        {
            var playlist = new Playlist
            {
                Name = dto.Name,
                CreatedAt = System.DateTimeOffset.UtcNow,
                UserId = GetCurrentUserId()
            };
            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();
            return playlist;
        }

        public async Task<bool> UpdateAsync(int id, Playlist updated)
        {
            var existing = await _context.Playlists.FindAsync(id);
            if (existing == null) return false;
            if (existing.UserId != GetCurrentUserId()) return false;

            existing.Name = updated.Name;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Playlists.FindAsync(id);
            if (existing == null) return false;
            if (existing.UserId != GetCurrentUserId()) return false;
            _context.Playlists.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddSongAsync(int playlistId, AddSongToPlaylist dto)
        {
            // Placeholder: since we haven't defined a relational structure, just return true
            var existing = await _context.Playlists.FindAsync(playlistId);
            if (existing == null) return false;
            var userId = GetCurrentUserId();
            if (existing.UserId != userId) return false;

            var exists = await _context.Songs.AnyAsync(s => s.Id == dto.SongId);
            if (!exists) return false;

            var linkExists = await _context.PlaylistSongs.AnyAsync(ps => ps.PlaylistId == playlistId && ps.SongId == dto.SongId);
            if (linkExists) return true;

            var link = new PlaylistSong
            {
                PlaylistId = playlistId,
                SongId = dto.SongId,
                Order = dto.Order
            };
            _context.PlaylistSongs.Add(link);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveSongAsync(int playlistId, int songId)
        {
            var existing = await _context.Playlists.FindAsync(playlistId);
            if (existing == null) return false;
            var userId = GetCurrentUserId();
            var isAdmin = _httpContextAccessor.HttpContext?.User?.IsInRole("Admin") == true;
            if (!isAdmin && existing.UserId != userId) return false;

            var link = await _context.PlaylistSongs.FirstOrDefaultAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);
            if (link == null) return false;
            _context.PlaylistSongs.Remove(link);
            await _context.SaveChangesAsync();
            return true;
        }

        private int GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                throw new UnauthorizedAccessException();
            }

            // Try standard NameIdentifier, then "sub" claim
            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? user.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(idClaim))
            {
                throw new UnauthorizedAccessException();
            }

            return int.Parse(idClaim);
        }
    }
}


