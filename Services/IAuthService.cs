using OneProject.Server.Generated;
using OneProject.Server.Models.DTOs;

namespace OneProject.Server.Services
{
    public interface IAuthService
    {
        Task<UserEntity> RegisterAsync(UserCreate payload, string role = "User");
        Task<TokenResponse?> LoginAsync(UserCreate payload);
        Task<TokenResponse?> RefreshAsync(RefreshRequest request);
    }
}


