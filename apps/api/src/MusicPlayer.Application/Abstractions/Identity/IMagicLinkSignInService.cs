using MusicPlayer.Application.Common;

namespace MusicPlayer.Application.Abstractions.Identity
{
    public interface IMagicLinkSignInService
    {
        Task<Result<Guid>> SignInAsync(string email, CancellationToken cancellationToken);
    }
}
