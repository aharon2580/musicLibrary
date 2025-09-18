using Microsoft.EntityFrameworkCore;
using OneProject.Server.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OneProject.Server.Generated;
using OneProject.Server.Models.DTOs;

var builder = WebApplication.CreateBuilder(args);

// ---- DbContext ----
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ---- CORS ---- ✅ הוספה כאן
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // כתובת ה-Frontend שלך
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "OneProject API", Version = "v1" });
    var bearerScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT token"
    };
    o.AddSecurityDefinition("bearerAuth", bearerScheme);
    o.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "bearerAuth"
                }
            }, new string[] {}
        }
    });
});
builder.Services.AddScoped<OneProject.Server.Services.ISongService, OneProject.Server.Services.SongService>();
builder.Services.AddScoped<OneProject.Server.Services.IPlaylistService, OneProject.Server.Services.PlaylistService>();
builder.Services.AddScoped<OneProject.Server.Services.IAuthService, OneProject.Server.Services.AuthService>();
builder.Services.AddHttpContextAccessor();

// ---- JWT Auth ----
var jwtKey = builder.Configuration["Jwt:Key"] ?? "dev-secret-key-please-change";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "oneproject";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "oneproject-clients";
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

// ---- CORS usage ---- ✅ חובה לשים לפני UseAuthentication/Authorization
app.UseCors("AllowAngular");

// ---- Dev admin seeding ----
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var auth = services.GetRequiredService<OneProject.Server.Services.IAuthService>();

        var adminUserName = builder.Configuration["Admin:UserName"] ?? "admin";
        var adminEmail = builder.Configuration["Admin:Email"] ?? "admin@example.com";
        var adminPassword = builder.Configuration["Admin:Password"] ?? "AdminPass123!";

        var adminExists = context.Users.Any(u => u.UserName == adminUserName);
        if (!adminExists)
        {
            await auth.RegisterAsync(new OneProject.Server.Generated.UserCreate
            {
                UserName = adminUserName,
                Email = adminEmail,
                Password = adminPassword
            }, role: "Admin");
        }

        // --- Sample data seeding (idempotent) ---
        if (!context.Songs.Any())
        {
            var now = DateTimeOffset.UtcNow;
            var songs = new List<Song>();
            for (int i = 1; i <= 20; i++)
            {
                songs.Add(new Song
                {
                    Title = $"Song {i}",
                    Artist = $"Artist {((i - 1) % 5) + 1}",
                    Album = $"Album {((i - 1) % 3) + 1}",
                    DurationSeconds = 120 + (i * 7) % 180,
                    StreamUrl = null,
                    CreatedAt = now
                });
            }
            context.Songs.AddRange(songs);
            await context.SaveChangesAsync();
        }

        // Create users, each with 1-2 playlists and 3-10 songs
        var userList = new[]
        {
            new { UserName = "Daniel Cohen", Email = "daniel.cohen@example.com", Password = "D@nC0hen!92w" },
            new { UserName = "Maya Levy", Email = "maya.levy@example.com", Password = "MayaL#2025x!" },
            new { UserName = "Noam Mizrahi", Email = "noam.mizrahi@example.com", Password = "Nm!zR4h1_7Q" },
            new { UserName = "Yael Adler", Email = "yael.adler@example.com", Password = "Y@elAdl3r%88" },
            new { UserName = "Omer Katz", Email = "omer.katz@example.com", Password = "0m3rKatz!5v" }
        };

        foreach (var userInfo in userList)
        {
            var u = context.Users.FirstOrDefault(x => x.UserName == userInfo.UserName);
            if (u == null)
            {
                u = await auth.RegisterAsync(new OneProject.Server.Generated.UserCreate
                {
                    UserName = userInfo.UserName,
                    Email = userInfo.Email,
                    Password = userInfo.Password
                }, role: "User");
            }

            // Load song ids
            var songIds = context.Songs.Select(s => s.Id).ToList();
            if (!songIds.Any()) continue;

            var rnd = new Random(userInfo.UserName.GetHashCode());
            int playlistsToCreate = 1 + rnd.Next(0, 2); // 1-2
            for (int p = 1; p <= playlistsToCreate; p++)
            {
                var pname = $"{userInfo.UserName}-playlist-{p}";
                var playlist = context.Playlists.FirstOrDefault(pl => pl.Name == pname && pl.UserId == u.Id);
                if (playlist == null)
                {
                    playlist = new Playlist
                    {
                        Name = pname,
                        UserId = u.Id,
                        CreatedAt = DateTimeOffset.UtcNow
                    };
                    context.Playlists.Add(playlist);
                    await context.SaveChangesAsync();
                }

                // Add 3-10 random songs
                int items = 3 + rnd.Next(0, 8);
                var chosen = songIds.OrderBy(_ => rnd.Next()).Take(items).ToList();
                int order = 0;
                foreach (var sid in chosen)
                {
                    bool existsLink = context.PlaylistSongs.Any(ps => ps.PlaylistId == playlist.Id && ps.SongId == sid);
                    if (existsLink) continue;
                    context.PlaylistSongs.Add(new PlaylistSong
                    {
                        PlaylistId = playlist.Id,
                        SongId = sid,
                        Order = order++
                    });
                }
                await context.SaveChangesAsync();
            }
        }
    }
    catch (Exception)
    {
    }
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
