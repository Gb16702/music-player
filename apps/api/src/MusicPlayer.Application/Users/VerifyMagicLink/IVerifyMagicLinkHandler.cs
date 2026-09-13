using MusicPlayer.Application.Common;

namespace MusicPlayer.Application.Users.VerifyMagicLink
{
    public interface IVerifyMagicLinkHandler
    {
        Task<Result<Guid>> HandleAsync(VerifyMagicLinkCommand command, CancellationToken cancellationToken);
    }
}
