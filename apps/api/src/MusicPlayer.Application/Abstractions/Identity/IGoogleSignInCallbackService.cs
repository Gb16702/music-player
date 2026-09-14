using MusicPlayer.Application.Common;

namespace MusicPlayer.Application.Abstractions.Identity
{
    public interface IGoogleSignInCallbackService
    {
        Task<Result<Guid>> CompleteSignInAsync(CancellationToken cancellationToken);
    }
}
