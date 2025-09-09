using System.ComponentModel.DataAnnotations;

namespace OneProject.Server.Models.DTOs
{
    public class PlaylistSong
    {
        public int Id { get; set; }

        [Required]
        public int PlaylistId { get; set; }

        [Required]
        public int SongId { get; set; }

        [Required]
        public int Order { get; set; }
    }
}


