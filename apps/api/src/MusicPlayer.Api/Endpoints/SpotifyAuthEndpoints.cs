using MusicPlayer.Application.Abstractions.Identity;

namespace MusicPlayer.Api.Endpoints;

internal static class SpotifyAuthEndpoints
{
    private const string SpotifyCallbackPath = "/api/v1/auth/spotify/callback";

    public static RouteGroupBuilder MapSpotifyAuthEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/auth/spotify/login", LoginWithSpotify)
            .WithName("LoginWithSpotify")
            .WithSummary("Starts Spotify OAuth sign-in.")
            .WithDescription("Redirects the user to Spotify for authentication.")
            .WithTags("Auth");

        group.MapGet("/auth/spotify/callback", SpotifyCallback)
            .WithName("SpotifyAuthCallback")
            .WithSummary("Handles the Spotify OAuth callback.")
            .WithDescription("Completes sign-in and redirects back to the web app.")
            .WithTags("Auth");

        return group;
    }

    private static Task LoginWithSpotify(
        HttpContext httpContext,
        ISpotifyLoginChallengeService spotifyLoginChallengeService,
        CancellationToken cancellationToken)
    {
        return spotifyLoginChallengeService.ChallengeAsync(httpContext, SpotifyCallbackPath, cancellationToken);
    }

    private static async Task<IResult> SpotifyCallback(
        ISpotifySignInCallbackService spotifySignInCallbackService,
        IWebAppRedirectBuilder webAppRedirectBuilder,
        CancellationToken cancellationToken)
    {
        var result = await spotifySignInCallbackService.CompleteSignInAsync(cancellationToken);

        if (!result.IsSuccess)
        {
            return Results.Redirect(
                webAppRedirectBuilder.BuildAuthCallbackErrorUrl(result.Error!.Code, result.Error.Message));
        }

        return Results.Redirect(webAppRedirectBuilder.BuildAuthCallbackSuccessUrl());
    }
}
