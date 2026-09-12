using MusicPlayer.Application.Common;

namespace MusicPlayer.Application.Users.Register
{
    public interface IRegisterUserHandler
    {
        Task<Result<Guid>> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken);
    }
}
