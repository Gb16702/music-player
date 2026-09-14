using MusicPlayer.Application.Users.SignInWithExternalProvider;

namespace MusicPlayer.Application.Abstractions.Persistence;

public interface IUserSpotifyTokenRepository
{
    Task UpsertAsync(Guid userId, SpotifyOAuthTokens tokens, CancellationToken cancellationToken);
}
