namespace MusicPlayer.Infrastructure.Spotify;

internal static class SpotifyAuthConstants
{
    public const string LoginProvider = "Spotify";

    public const string OAuthCallbackPath = "/signin-spotify";

    public const string ApplicationCallbackPath = "/api/v1/auth/spotify/callback";
}
