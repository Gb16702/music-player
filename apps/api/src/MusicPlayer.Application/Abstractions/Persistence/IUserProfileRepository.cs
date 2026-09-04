using MusicPlayer.Domain.Users;

namespace MusicPlayer.Application.Abstractions.Persistence
{
    public interface IUserProfileRepository
    {
        void Add(UserProfile profile);
    }
}
