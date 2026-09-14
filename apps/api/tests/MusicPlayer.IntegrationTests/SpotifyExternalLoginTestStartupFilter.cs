using System.Security.Claims;
using AspNet.Security.OAuth.Spotify;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MusicPlayer.Infrastructure.Identity;

namespace MusicPlayer.IntegrationTests;

internal sealed class SpotifyExternalLoginTestStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            app.Use(async (context, nextMiddleware) =>
            {
                if (context.Request.Path.Equals("/integration-test/seed-spotify-external"))
                {
                    var signInManager = context.RequestServices.GetRequiredService<SignInManager<ApplicationUser>>();
                    var email = context.Request.Query["email"].ToString();
                    var key = context.Request.Query["key"].ToString();

                    var properties = signInManager.ConfigureExternalAuthenticationProperties(
                        SpotifyAuthenticationDefaults.AuthenticationScheme,
                        "/api/v1/auth/spotify/callback");

                    properties.StoreTokens(
                    [
                        new AuthenticationToken { Name = "access_token", Value = "integration-access-token" },
                        new AuthenticationToken { Name = "refresh_token", Value = "integration-refresh-token" }
                    ]);

                    var identity = new ClaimsIdentity(
                        [
                            new Claim(ClaimTypes.Email, email),
                            new Claim(ClaimTypes.NameIdentifier, key)
                        ],
                        SpotifyAuthenticationDefaults.AuthenticationScheme);

                    await context.SignInAsync(
                        IdentityConstants.ExternalScheme,
                        new ClaimsPrincipal(identity),
                        properties);

                    context.Response.StatusCode = StatusCodes.Status200OK;

                    return;
                }

                await nextMiddleware();
            });

            next(app);
        };
    }
}
