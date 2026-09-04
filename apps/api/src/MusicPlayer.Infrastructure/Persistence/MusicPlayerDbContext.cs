using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusicPlayer.Domain.Users;
using MusicPlayer.Infrastructure.Identity;

namespace MusicPlayer.Infrastructure.Persistence
{
    internal sealed class MusicPlayerDbContext : IdentityUserContext<ApplicationUser, Guid>
    {
        public MusicPlayerDbContext(DbContextOptions<MusicPlayerDbContext> options) : base(options) { }

        public DbSet<UserProfile> UserProfiles
        {
            get
            {
                return Set<UserProfile>();
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(MusicPlayerDbContext).Assembly);
        }
    }
}
