using MusicPlayer.Application.Common;

namespace MusicPlayer.Application.Abstractions.Identity
{
    public interface IIdentityService
    {
        Task<Result<Guid>> CreateUserAsync(string email, string password, CancellationToken cancellationToken);
    }
}
