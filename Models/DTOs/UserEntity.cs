using System.ComponentModel.DataAnnotations;

namespace OneProject.Server.Models.DTOs
{
    public class UserEntity
    {
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

        [Required]
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

        // PBKDF2 iteration count for this user
        public int PasswordIterations { get; set; } = 100_000;

        [Required]
        public string Role { get; set; } = "User"; // User or Admin

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}


