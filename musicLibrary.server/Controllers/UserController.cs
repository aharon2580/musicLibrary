using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneProject.Server.Generated;
using OneProject.Server.Services;

namespace OneProject.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _auth;

        public UserController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCreate user)
        {
            var created = await _auth.RegisterAsync(user);
            return Ok(new UserPublic { Id = created.Id, UserName = created.UserName, Email = created.Email, CreatedAt = created.CreatedAt });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin user)
        {
            var token = await _auth.LoginAsync(user);
            if (token == null) return Unauthorized();
            return Ok(token);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            var result = await _auth.RefreshAsync(request);
            if (result == null) return Unauthorized();
            return Ok(result);
        }
    }
}