using Microsoft.EntityFrameworkCore;
using OneProject.Server.Data;
using OneProject.Server.Generated;

namespace OneProject.Server.Services
{
    public class SongService : ISongService
    {
        private readonly AppDbContext _context;

        public SongService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Song>> GetAllAsync()
        {
            return await _context.Songs.AsNoTracking().ToListAsync();
        }

        public async Task<Song?> GetByIdAsync(int id)
        {
            return await _context.Songs.FindAsync(id);
        }

        public async Task<Song> CreateAsync(CreateSong dto)
        {
            var song = new Song
            {
                Title = dto.Title,
                Artist = dto.Artist,
                Album = dto.Album,
                DurationSeconds = dto.DurationSeconds,
                StreamUrl = dto.StreamUrl,
                CreatedAt = System.DateTimeOffset.UtcNow
            };
            _context.Songs.Add(song);
            await _context.SaveChangesAsync();
            return song;
        }

        public async Task<bool> UpdateAsync(int id, Song updated)
        {
            var existing = await _context.Songs.FindAsync(id);
            if (existing == null) return false;

            existing.Title = updated.Title;
            existing.Artist = updated.Artist;
            existing.Album = updated.Album;
            existing.DurationSeconds = updated.DurationSeconds;
            existing.StreamUrl = updated.StreamUrl;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Songs.FindAsync(id);
            if (existing == null) return false;
            _context.Songs.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}


