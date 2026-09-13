using MusicPlayer.Application.Common;

namespace MusicPlayer.Application.Users.RequestMagicLink
{
    public interface IRequestMagicLinkHandler
    {
        Task<Result<bool>> HandleAsync(RequestMagicLinkCommand command, CancellationToken cancellationToken);
    }
}
