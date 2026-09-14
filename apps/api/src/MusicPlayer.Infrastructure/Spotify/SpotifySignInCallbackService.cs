using System.Globalization;
using System.Security.Claims;
using AspNet.Security.OAuth.Spotify;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MusicPlayer.Application.Abstractions.Identity;
using MusicPlayer.Application.Common;
using MusicPlayer.Application.Users.SignInWithExternalProvider;
using MusicPlayer.Infrastructure.Identity;

namespace MusicPlayer.Infrastructure.Spotify;

internal sealed class SpotifySignInCallbackService : ISpotifySignInCallbackService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ISignInWithExternalProviderHandler _signInWithExternalProviderHandler;
    private readonly ILogger<SpotifySignInCallbackService> _logger;

    public SpotifySignInCallbackService(
        IHttpContextAccessor httpContextAccessor,
        SignInManager<ApplicationUser> signInManager,
        ISignInWithExternalProviderHandler signInWithExternalProviderHandler,
        ILogger<SpotifySignInCallbackService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _signInManager = signInManager;
        _signInWithExternalProviderHandler = signInWithExternalProviderHandler;
        _logger = logger;
    }

    public async Task<Result<Guid>> CompleteSignInAsync(CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext is null)
        {
            return Result<Guid>.Failure(ExternalAuthErrors.SignInFailed("HTTP context is unavailable."));
        }

        var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();

        if (externalLoginInfo is null)
        {
            var externalAuth = await httpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);

            _logger.LogWarning(
                "Spotify callback missing external login info. ExternalSchemeSucceeded={Succeeded}, Failure={Failure}",
                externalAuth.Succeeded,
                externalAuth.Failure?.Message);

            return Result<Guid>.Failure(ExternalAuthErrors.SignInFailed("External login information is unavailable."));
        }

        var email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email)
            ?? externalLoginInfo.Principal.FindFirstValue("email");

        if (string.IsNullOrWhiteSpace(email))
        {
            await httpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            return Result<Guid>.Failure(ExternalAuthErrors.EmailRequired());
        }

        var accessToken = externalLoginInfo.AuthenticationTokens
            .FirstOrDefault(token => token.Name == "access_token")
            ?.Value;

        var refreshToken = externalLoginInfo.AuthenticationTokens
            .FirstOrDefault(token => token.Name == "refresh_token")
            ?.Value;

        if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(refreshToken))
        {
            await httpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            return Result<Guid>.Failure(
                ExternalAuthErrors.SignInFailed("Spotify access or refresh token is unavailable."));
        }

        DateTimeOffset? accessTokenExpiresAt = null;
        var expiresAt = externalLoginInfo.AuthenticationTokens
            .FirstOrDefault(token => token.Name == "expires_at")
            ?.Value;

        if (!string.IsNullOrWhiteSpace(expiresAt)
            && DateTimeOffset.TryParse(expiresAt, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed))
        {
            accessTokenExpiresAt = parsed;
        }

        var spotifyTokens = new SpotifyOAuthTokens(accessToken, refreshToken, accessTokenExpiresAt);

        var result = await _signInWithExternalProviderHandler.HandleAsync(
            new SignInWithExternalProviderCommand(
                email.Trim(),
                externalLoginInfo.LoginProvider,
                externalLoginInfo.ProviderKey,
                spotifyTokens),
            cancellationToken);

        await httpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        return result;
    }
}
