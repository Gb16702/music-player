using MusicPlayer.Application.Common;

namespace MusicPlayer.Application.Users.GetCurrentUser
{
    public interface IGetCurrentUserHandler
    {
        Task<Result<CurrentUser>> HandleAsync(GetCurrentUserQuery query, CancellationToken cancellationToken);
    }
}
