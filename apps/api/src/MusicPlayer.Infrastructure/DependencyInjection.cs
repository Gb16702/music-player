using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicPlayer.Infrastructure.Identity;
using MusicPlayer.Infrastructure.Persistence;
using MusicPlayer.Infrastructure.Spotify;

namespace MusicPlayer.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Database");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Database connection string is required");
            }

            services.AddDbContext<MusicPlayerDbContext>(optionsBuilder => optionsBuilder.UseNpgsql(connectionString));

            var identityBuilder = services.AddIdentityCore<ApplicationUser>();

            identityBuilder.AddEntityFrameworkStores<MusicPlayerDbContext>();

            services.AddOptions<SpotifyOptions>()
                .Bind(configuration.GetSection(SpotifyOptions.SectionName))
                .Validate(options => !string.IsNullOrWhiteSpace(options.ClientId), "Spotify:ClientId is required")
                .Validate(options => !string.IsNullOrWhiteSpace(options.ClientSecret), "Spotify:ClientSecret is required")
                .ValidateOnStart();

            return services;
        }
    }
}
