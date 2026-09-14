using MusicPlayer.Application.Common;

namespace MusicPlayer.Application.Users.SignInWithExternalProvider
{
    public interface ISignInWithExternalProviderHandler
    {
        Task<Result<Guid>> HandleAsync(SignInWithExternalProviderCommand command, CancellationToken cancellationToken);
    }
}
