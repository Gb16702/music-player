using MusicPlayer.Application.Common;

namespace MusicPlayer.Application.Abstractions.Identity
{
    public interface IExternalAuthSignInService
    {
        Task<Result<Guid>> SignInAsync(
            string email,
            string loginProvider,
            string providerKey,
            CancellationToken cancellationToken);
    }
}
