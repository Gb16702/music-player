namespace MusicPlayer.Application.Users.Logout
{
    public interface ILogoutUserHandler
    {
        Task HandleAsync(CancellationToken cancellationToken);
    }
}
