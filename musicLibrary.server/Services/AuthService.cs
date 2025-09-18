using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OneProject.Server.Data;
using OneProject.Server.Generated;
using OneProject.Server.Models.DTOs;

namespace OneProject.Server.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<UserEntity> RegisterAsync(UserCreate payload, string role = "User")
        {
            var exists = await _context.Users.AnyAsync(u => u.UserName == payload.UserName || u.Email == payload.Email);
            if (exists)
            {
                throw new InvalidOperationException("User already exists");
            }

            CreatePasswordHash(payload.Password, out var hash, out var salt, out var iterations);
            var user = new UserEntity
            {
                UserName = payload.UserName,
                Email = payload.Email,
                PasswordHash = hash,
                PasswordSalt = salt,
                PasswordIterations = iterations,
                Role = role
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<TokenResponse?> LoginAsync(UserLogin payload)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == payload.Email);
            if (user == null) return null;
            // Support legacy users with iterations == 0 (HMACSHA256), and upgrade on successful login
            if (user.PasswordIterations <= 0)
            {
                if (!VerifyLegacyPassword(payload.Password, user.PasswordHash, user.PasswordSalt)) return null;
                // upgrade to PBKDF2
                CreatePasswordHash(payload.Password, out var newHash, out var newSalt, out var newIterations);
                user.PasswordHash = newHash;
                user.PasswordSalt = newSalt;
                user.PasswordIterations = newIterations;
                await _context.SaveChangesAsync();
            }
            else
            {
                if (!VerifyPassword(payload.Password, user.PasswordHash, user.PasswordSalt, user.PasswordIterations)) return null;
            }

            var (token, expiresIn) = GenerateJwt(user);
            var refresh = await CreateRefreshTokenAsync(user.Id, null);
            return new TokenResponse { AccessToken = token, ExpiresIn = expiresIn, RefreshToken = refresh };
        }

        public async Task<TokenResponse?> RefreshAsync(RefreshRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken)) return null;
            var rt = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == request.RefreshToken);
            if (rt == null || !rt.IsActive || rt.ExpiresAt <= DateTimeOffset.UtcNow || rt.RevokedAt != null)
            {
                return null;
            }

            var user = await _context.Users.FindAsync(rt.UserId);
            if (user == null) return null;

            // rotate: revoke old, create new
            rt.IsActive = false;
            rt.RevokedAt = DateTimeOffset.UtcNow;
            var newToken = await CreateRefreshTokenAsync(user.Id, request.DeviceId);

            var (access, expiresIn) = GenerateJwt(user);
            await _context.SaveChangesAsync();
            return new TokenResponse { AccessToken = access, ExpiresIn = expiresIn, RefreshToken = newToken };
        }

        private async Task<string> CreateRefreshTokenAsync(int userId, string? deviceId)
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var rt = new Models.DTOs.RefreshToken
            {
                UserId = userId,
                Token = token,
                DeviceId = deviceId,
                CreatedAt = DateTimeOffset.UtcNow,
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(14),
                IsActive = true
            };
            _context.RefreshTokens.Add(rt);
            await _context.SaveChangesAsync();
            return token;
        }

        private static void CreatePasswordHash(string password, out byte[] hash, out byte[] salt, out int iterations)
        {
            salt = RandomNumberGenerator.GetBytes(16);
            iterations = 100_000;
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            hash = pbkdf2.GetBytes(32);
        }

        private static bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt, int iterations)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, storedSalt, iterations, HashAlgorithmName.SHA256);
            var computed = pbkdf2.GetBytes(32);
            return computed.SequenceEqual(storedHash);
        }

        private static bool VerifyLegacyPassword(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA256(storedSalt);
            var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computed.SequenceEqual(storedHash);
        }

        private (string token, int expiresIn) GenerateJwt(UserEntity user)
        {
            var key = _config["Jwt:Key"] ?? "dev-secret-key-please-change";
            var issuer = _config["Jwt:Issuer"] ?? "oneproject";
            var audience = _config["Jwt:Audience"] ?? "oneproject-clients";
            var expires = DateTime.UtcNow.AddHours(1);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            var expiresInSeconds = (int)TimeSpan.FromHours(1).TotalSeconds;
            return (tokenString, expiresInSeconds);
        }
    }
}


