using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicPlayer.Application.Abstractions.Identity;
using MusicPlayer.Application.Abstractions.Persistence;
using MusicPlayer.Infrastructure.Email;
using MusicPlayer.Infrastructure.Identity;
using MusicPlayer.Infrastructure.Persistence;
using MusicPlayer.Infrastructure.Persistence.Repositories;
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

            services.AddDataProtection();
            services.AddEmail(configuration);

            var identityBuilder = services.AddIdentityCore<ApplicationUser>();

            identityBuilder
                .AddEntityFrameworkStores<MusicPlayerDbContext>()
                .AddSignInManager();

            services.AddAuthentication(IdentityConstants.ApplicationScheme)
                .AddCookie(IdentityConstants.ApplicationScheme, options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.ExpireTimeSpan = TimeSpan.FromDays(14);
                    options.SlidingExpiration = true;
                });

            services.AddAuthorization();

            services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
            });

            services.AddScoped<IUserProfileRepository, UserProfileRepository>();

            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<MusicPlayerDbContext>());

            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IAuthSignInService, AuthSignInService>();
            services.AddScoped<IAuthSignOutService, AuthSignOutService>();
            services.AddScoped<IUserAccountReader, UserAccountReader>();
            services.AddScoped<IMagicLinkTokenService, MagicLinkTokenService>();
            services.AddScoped<IMagicLinkUrlBuilder, MagicLinkUrlBuilder>();
            services.AddScoped<IMagicLinkSignInService, MagicLinkSignInService>();

            services.AddOptions<SpotifyOptions>()
                .Bind(configuration.GetSection(SpotifyOptions.SectionName))
                .Validate(options => !string.IsNullOrWhiteSpace(options.ClientId), "Spotify:ClientId is required")
                .Validate(options => !string.IsNullOrWhiteSpace(options.ClientSecret), "Spotify:ClientSecret is required")
                .ValidateOnStart();

            return services;
        }
    }
}
