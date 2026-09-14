using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MusicPlayer.Infrastructure.Identity;

namespace MusicPlayer.IntegrationTests;

internal sealed class GoogleExternalLoginTestStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            app.Use(async (context, nextMiddleware) =>
            {
                if (context.Request.Path.Equals("/integration-test/seed-google-external"))
                {
                    var signInManager = context.RequestServices.GetRequiredService<SignInManager<ApplicationUser>>();
                    var email = context.Request.Query["email"].ToString();
                    var key = context.Request.Query["key"].ToString();

                    var properties = signInManager.ConfigureExternalAuthenticationProperties(
                        GoogleDefaults.AuthenticationScheme,
                        "/api/v1/auth/google/callback");

                    var identity = new ClaimsIdentity(
                        [
                            new Claim(ClaimTypes.Email, email),
                            new Claim(ClaimTypes.NameIdentifier, key)
                        ],
                        GoogleDefaults.AuthenticationScheme);

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
