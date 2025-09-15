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
    public class SongsController : ControllerBase
    {
        private readonly ISongService _songs;

        public SongsController(ISongService songs)
        {
            _songs = songs;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _songs.GetAllAsync();
            return Ok(all);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var song = await _songs.GetByIdAsync(id);
            if (song == null) return NotFound();
            return Ok(song);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateSong dto)
        {
            var song = await _songs.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = song.Id }, song);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] Song updatedSong)
        {
            var ok = await _songs.UpdateAsync(id, updatedSong);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _songs.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}