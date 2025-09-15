using OneProject.Server.Generated;

namespace OneProject.Server.Services
{
    public interface IPlaylistService
    {
        Task<List<Playlist>> GetAllAsync();
        Task<Playlist?> GetByIdAsync(int id);
        Task<Playlist> CreateAsync(CreatePlaylist dto);
        Task<bool> UpdateAsync(int id, Playlist updated);
        Task<bool> DeleteAsync(int id);
        Task<bool> AddSongAsync(int playlistId, AddSongToPlaylist dto);
        Task<bool> RemoveSongAsync(int playlistId, int songId);
    }
}


