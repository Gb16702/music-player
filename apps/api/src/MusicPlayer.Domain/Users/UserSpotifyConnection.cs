namespace MusicPlayer.Domain.Users;

public sealed class UserSpotifyConnection
{
    public Guid UserId { get; private set; }

    public string AccessToken { get; private set; } = string.Empty;

    public string RefreshToken { get; private set; } = string.Empty;

    public DateTimeOffset? AccessTokenExpiresAt { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }

    private UserSpotifyConnection()
    {
    }

    public UserSpotifyConnection(
        Guid userId,
        string accessToken,
        string refreshToken,
        DateTimeOffset? accessTokenExpiresAt)
    {
        UserId = userId;
        UpdateTokens(accessToken, refreshToken, accessTokenExpiresAt);
    }

    public void UpdateTokens(string accessToken, string refreshToken, DateTimeOffset? accessTokenExpiresAt)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        AccessTokenExpiresAt = accessTokenExpiresAt;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
}
