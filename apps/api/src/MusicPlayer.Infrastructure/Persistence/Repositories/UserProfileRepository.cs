using MusicPlayer.Application.Abstractions.Persistence;
using MusicPlayer.Domain.Users;

namespace MusicPlayer.Infrastructure.Persistence.Repositories
{
    internal sealed class UserProfileRepository : IUserProfileRepository
    {
        private readonly MusicPlayerDbContext _dbContext;

        public UserProfileRepository(MusicPlayerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(UserProfile profile)
        {
            _dbContext.UserProfiles.Add(profile);
        }
    }
}
