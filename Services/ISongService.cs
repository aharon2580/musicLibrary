using OneProject.Server.Generated;

namespace OneProject.Server.Services
{
    public interface ISongService
    {
        Task<List<Song>> GetAllAsync();
        Task<Song?> GetByIdAsync(int id);
        Task<Song> CreateAsync(CreateSong dto);
        Task<bool> UpdateAsync(int id, Song updated);
        Task<bool> DeleteAsync(int id);
    }
}


