namespace MusicPlayer.Application.Abstractions.Persistence
{
    public interface IUnitOfWork
    {
        Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
