using MusicPlayer.Application.Common;

namespace MusicPlayer.Application.Abstractions.Identity;

public interface ISpotifySignInCallbackService
{
    Task<Result<Guid>> CompleteSignInAsync(CancellationToken cancellationToken);
}
