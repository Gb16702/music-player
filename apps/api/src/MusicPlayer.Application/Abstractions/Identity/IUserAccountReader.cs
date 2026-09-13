namespace MusicPlayer.Application.Abstractions.Identity
{
    public interface IUserAccountReader
    {
        Task<string?> GetEmailAsync(Guid userId, CancellationToken cancellationToken);
    }
}
