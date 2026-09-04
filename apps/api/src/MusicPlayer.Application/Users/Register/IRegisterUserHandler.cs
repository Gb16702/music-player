namespace MusicPlayer.Application.Users.Register
{
    public interface IRegisterUserHandler
    {
        Task<Guid> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken);
    }
}
