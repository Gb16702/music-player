namespace MusicPlayer.Application.Abstractions.Identity
{
    public interface IAuthSignOutService
    {
        Task SignOutAsync(CancellationToken cancellationToken);
    }
}
