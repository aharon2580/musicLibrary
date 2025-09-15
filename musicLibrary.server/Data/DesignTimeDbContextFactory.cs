using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace OneProject.Server.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            // Add development settings only if the file exists to avoid design-time failures
            var devSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Development.json");
            if (File.Exists(devSettingsPath))
            {
                builder.AddJsonFile("appsettings.Development.json", optional: true);
            }

            var configuration = builder.AddEnvironmentVariables().Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}


