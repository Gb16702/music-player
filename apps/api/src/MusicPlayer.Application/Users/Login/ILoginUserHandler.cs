using MusicPlayer.Application.Common;

namespace MusicPlayer.Application.Users.Login
{
    public interface ILoginUserHandler
    {
        Task<Result<Guid>> HandleAsync(LoginUserCommand command, CancellationToken cancellationToken);
    }
}
