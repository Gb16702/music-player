using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MusicPlayer.Application.Abstractions.Identity;
using MusicPlayer.Application.Abstractions.Persistence;
using MusicPlayer.Application.Users.SignInWithExternalProvider;
using MusicPlayer.Infrastructure.Email;
using MusicPlayer.Infrastructure.Google;
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

            var dataProtectionKeysPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MusicPlayer",
                "data-protection-keys");

            Directory.CreateDirectory(dataProtectionKeysPath);

            services.AddDataProtection()
                .SetApplicationName("MusicPlayer.Api")
                .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysPath));

            services.AddEmail(configuration);
            services.AddHttpContextAccessor();

            var identityBuilder = services.AddIdentityCore<ApplicationUser>();

            identityBuilder
                .AddEntityFrameworkStores<MusicPlayerDbContext>()
                .AddSignInManager();

            services.AddOptions<GoogleAuthOptions>()
                .Bind(configuration.GetSection(GoogleAuthOptions.SectionName))
                .Validate(options => !string.IsNullOrWhiteSpace(options.ClientId), "Google:ClientId is required")
                .Validate(options => !string.IsNullOrWhiteSpace(options.ClientSecret), "Google:ClientSecret is required")
                .ValidateOnStart();

            var googleClientId = configuration["Google:ClientId"];
            var googleClientSecret = configuration["Google:ClientSecret"];

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddCookie(IdentityConstants.ApplicationScheme, options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.Cookie.Path = "/";
                    options.ExpireTimeSpan = TimeSpan.FromDays(14);
                    options.SlidingExpiration = true;
                })
                .AddCookie(IdentityConstants.ExternalScheme, options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.Cookie.Path = "/";
                    options.Cookie.IsEssential = true;
                })
                .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
                {
                    options.ClientId = googleClientId!;
                    options.ClientSecret = googleClientSecret!;
                    options.CallbackPath = GoogleAuthConstants.OAuthCallbackPath;
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                    options.CorrelationCookie.HttpOnly = true;
                    options.CorrelationCookie.IsEssential = true;
                    options.CorrelationCookie.Path = "/";
                    options.CorrelationCookie.SameSite = SameSiteMode.Lax;
                    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.Events.OnRemoteFailure = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILoggerFactory>()
                            .CreateLogger("GoogleAuthentication");

                        logger.LogWarning(
                            context.Failure,
                            "Google remote authentication failed.");

                        context.HandleResponse();

                        var redirectBuilder = context.HttpContext.RequestServices
                            .GetRequiredService<IWebAppRedirectBuilder>();

                        context.Response.Redirect(
                            redirectBuilder.BuildAuthCallbackErrorUrl(
                                ExternalAuthErrors.SignInFailedCode,
                                context.Failure?.Message));

                        return Task.CompletedTask;
                    };
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
            services.AddScoped<IExternalAuthSignInService, ExternalAuthSignInService>();
            services.AddScoped<IGoogleLoginChallengeService, GoogleLoginChallengeService>();
            services.AddScoped<IGoogleSignInCallbackService, GoogleSignInCallbackService>();
            services.AddScoped<IWebAppRedirectBuilder, WebAppRedirectBuilder>();

            services.AddOptions<SpotifyOptions>()
                .Bind(configuration.GetSection(SpotifyOptions.SectionName))
                .Validate(options => !string.IsNullOrWhiteSpace(options.ClientId), "Spotify:ClientId is required")
                .Validate(options => !string.IsNullOrWhiteSpace(options.ClientSecret), "Spotify:ClientSecret is required")
                .ValidateOnStart();

            return services;
        }
    }
}
