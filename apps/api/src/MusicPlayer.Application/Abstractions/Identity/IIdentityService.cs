namespace MusicPlayer.Application.Abstractions.Identity
{
    public interface IIdentityService
    {
        Task<Guid> CreateUserAsync(string email, string password, CancellationToken cancellationToken);
    }
}
