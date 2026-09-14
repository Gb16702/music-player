using MusicPlayer.Application.Common;
using MusicPlayer.Application.Users.SignInWithExternalProvider;

namespace MusicPlayer.Application.Abstractions.Identity
{
    public interface IExternalAuthSignInService
    {
        Task<Result<Guid>> SignInAsync(
            string email,
            string loginProvider,
            string providerKey,
            SpotifyOAuthTokens? spotifyTokens,
            CancellationToken cancellationToken);
    }
}
