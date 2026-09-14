namespace MusicPlayer.Application.Users.SignInWithExternalProvider;

public sealed record SpotifyOAuthTokens(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset? AccessTokenExpiresAt);
