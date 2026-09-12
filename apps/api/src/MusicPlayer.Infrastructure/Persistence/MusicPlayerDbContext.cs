using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusicPlayer.Application.Abstractions.Persistence;
using MusicPlayer.Domain.Users;
using MusicPlayer.Infrastructure.Identity;

namespace MusicPlayer.Infrastructure.Persistence
{
    internal sealed class MusicPlayerDbContext : IdentityUserContext<ApplicationUser, Guid>, IUnitOfWork
    {
        public MusicPlayerDbContext(DbContextOptions<MusicPlayerDbContext> options) : base(options) { }

        public DbSet<UserProfile> UserProfiles
        {
            get
            {
                return Set<UserProfile>();
            }
        }

        public Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return BeginTransactionInternalAsync(cancellationToken);
        }

        public new Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(MusicPlayerDbContext).Assembly);
        }

        private async Task<IUnitOfWorkTransaction> BeginTransactionInternalAsync(CancellationToken cancellationToken)
        {
            var transaction = await Database.BeginTransactionAsync(cancellationToken);

            return new EfUnitOfWorkTransaction(transaction);
        }
    }
}
