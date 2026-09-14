using MusicPlayer.Application.Abstractions.Identity;

namespace MusicPlayer.Api.Endpoints;

internal static class GoogleAuthEndpoints
{
    private const string GoogleCallbackPath = "/api/v1/auth/google/callback";

    public static RouteGroupBuilder MapGoogleAuthEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/auth/google/login", LoginWithGoogle)
            .WithName("LoginWithGoogle")
            .WithSummary("Starts Google OAuth sign-in.")
            .WithDescription("Redirects the user to Google for authentication.")
            .WithTags("Auth");

        group.MapGet("/auth/google/callback", GoogleCallback)
            .WithName("GoogleAuthCallback")
            .WithSummary("Handles the Google OAuth callback.")
            .WithDescription("Completes sign-in and redirects back to the web app.")
            .WithTags("Auth");

        return group;
    }

    private static Task LoginWithGoogle(
        HttpContext httpContext,
        IGoogleLoginChallengeService googleLoginChallengeService,
        CancellationToken cancellationToken)
    {
        return googleLoginChallengeService.ChallengeAsync(httpContext, GoogleCallbackPath, cancellationToken);
    }

    private static async Task<IResult> GoogleCallback(
        IGoogleSignInCallbackService googleSignInCallbackService,
        IWebAppRedirectBuilder webAppRedirectBuilder,
        CancellationToken cancellationToken)
    {
        var result = await googleSignInCallbackService.CompleteSignInAsync(cancellationToken);

        if (!result.IsSuccess)
        {
            return Results.Redirect(
                webAppRedirectBuilder.BuildAuthCallbackErrorUrl(result.Error!.Code, result.Error.Message));
        }

        return Results.Redirect(webAppRedirectBuilder.BuildAuthCallbackSuccessUrl());
    }
}
