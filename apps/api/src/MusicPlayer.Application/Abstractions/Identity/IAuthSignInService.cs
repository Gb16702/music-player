using MusicPlayer.Application.Common;

namespace MusicPlayer.Application.Abstractions.Identity
{
    public interface IAuthSignInService
    {
        Task<Result<Guid>> SignInAsync(string email, string password, bool isPersistent, CancellationToken cancellationToken);
    }
}
