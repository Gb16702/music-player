using MusicPlayer.Application.Abstractions.Identity;
using MusicPlayer.Application.Abstractions.Persistence;
using MusicPlayer.Application.Common;

namespace MusicPlayer.Application.Users.GetCurrentUser
{
    public sealed class GetCurrentUserHandler : IGetCurrentUserHandler
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUserAccountReader _userAccountReader;

        public GetCurrentUserHandler(IUserProfileRepository userProfileRepository, IUserAccountReader userAccountReader)
        {
            _userProfileRepository = userProfileRepository;
            _userAccountReader = userAccountReader;
        }

        public async Task<Result<CurrentUser>> HandleAsync(GetCurrentUserQuery query, CancellationToken cancellationToken)
        {
            var profile = await _userProfileRepository.GetByUserIdAsync(query.UserId, cancellationToken);

            if (profile is null)
            {
                return Result<CurrentUser>.Failure(GetCurrentUserErrors.UserNotFound());
            }

            var email = await _userAccountReader.GetEmailAsync(query.UserId, cancellationToken);

            if (email is null)
            {
                return Result<CurrentUser>.Failure(GetCurrentUserErrors.UserNotFound());
            }

            return Result<CurrentUser>.Success(
                new CurrentUser(
                    query.UserId,
                    email,
                    profile.DisplayName,
                    profile.AvatarUrl,
                    profile.OnboardingCompleted));
        }
    }
}
