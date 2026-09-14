using AspNet.Security.OAuth.Spotify;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MusicPlayer.Application.Abstractions.Identity;
using MusicPlayer.Infrastructure.Identity;

namespace MusicPlayer.Infrastructure.Spotify;

internal sealed class SpotifyLoginChallengeService : ISpotifyLoginChallengeService
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public SpotifyLoginChallengeService(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public Task ChallengeAsync(HttpContext httpContext, string redirectUrl, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var properties = _signInManager.ConfigureExternalAuthenticationProperties(
            SpotifyAuthenticationDefaults.AuthenticationScheme,
            redirectUrl);

        return httpContext.ChallengeAsync(SpotifyAuthenticationDefaults.AuthenticationScheme, properties);
    }
}
