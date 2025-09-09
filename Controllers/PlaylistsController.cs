using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneProject.Server.Data;
using OneProject.Server.Generated;
using OneProject.Server.Services;

namespace OneProject.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PlaylistsController : ControllerBase
    {
        private readonly IPlaylistService _playlists;

        public PlaylistsController(IPlaylistService playlists)
        {
            _playlists = playlists;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _playlists.GetAllAsync();
            return Ok(all);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var playlist = await _playlists.GetByIdAsync(id);
            if (playlist == null) return NotFound();
            return Ok(playlist);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePlaylist dto)
        {
            var playlist = await _playlists.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = playlist.Id }, playlist);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Playlist updatedPlaylist)
        {
            var ok = await _playlists.UpdateAsync(id, updatedPlaylist);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _playlists.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPost("{id}/add-song")]
        public async Task<IActionResult> AddSong(int id, [FromBody] AddSongToPlaylist dto)
        {
            var ok = await _playlists.AddSongAsync(id, dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}/songs/{songId}")]
        public async Task<IActionResult> RemoveSong(int id, int songId)
        {
            var ok = await _playlists.RemoveSongAsync(id, songId);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}