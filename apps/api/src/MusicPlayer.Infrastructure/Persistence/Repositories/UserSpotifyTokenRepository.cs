using Microsoft.EntityFrameworkCore;
using MusicPlayer.Application.Abstractions.Persistence;
using MusicPlayer.Application.Users.SignInWithExternalProvider;
using MusicPlayer.Domain.Users;

namespace MusicPlayer.Infrastructure.Persistence.Repositories;

internal sealed class UserSpotifyTokenRepository : IUserSpotifyTokenRepository
{
    private readonly MusicPlayerDbContext _dbContext;

    public UserSpotifyTokenRepository(MusicPlayerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task UpsertAsync(Guid userId, SpotifyOAuthTokens tokens, CancellationToken cancellationToken)
    {
        var existing = await _dbContext.UserSpotifyConnections
            .SingleOrDefaultAsync(connection => connection.UserId == userId, cancellationToken);

        if (existing is null)
        {
            _dbContext.UserSpotifyConnections.Add(
                new UserSpotifyConnection(
                    userId,
                    tokens.AccessToken,
                    tokens.RefreshToken,
                    tokens.AccessTokenExpiresAt));

            return;
        }

        existing.UpdateTokens(tokens.AccessToken, tokens.RefreshToken, tokens.AccessTokenExpiresAt);
    }
}
