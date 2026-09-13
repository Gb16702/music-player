using MusicPlayer.Application.Abstractions.Persistence;
using MusicPlayer.Application.Common;
using MusicPlayer.Domain.Users;

namespace MusicPlayer.Application.Users.CompleteOnboarding
{
    public sealed class CompleteOnboardingHandler : ICompleteOnboardingHandler
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CompleteOnboardingHandler(IUserProfileRepository userProfileRepository, IUnitOfWork unitOfWork)
        {
            _userProfileRepository = userProfileRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<OnboardingProfile>> HandleAsync(CompleteOnboardingCommand command, CancellationToken cancellationToken)
        {
            var profile = await _userProfileRepository.GetByUserIdAsync(command.UserId, cancellationToken);

            if (profile is null)
            {
                return Result<OnboardingProfile>.Failure(CompleteOnboardingErrors.ProfileNotFound());
            }

            if (profile.OnboardingCompleted)
            {
                return Result<OnboardingProfile>.Failure(CompleteOnboardingErrors.AlreadyCompleted());
            }

            try
            {
                profile.CompleteOnboarding(command.DisplayName, command.AvatarUrl);
            }
            catch (ArgumentException exception)
            {
                return Result<OnboardingProfile>.Failure(MapValidationError(exception));
            }
            catch (InvalidOperationException)
            {
                return Result<OnboardingProfile>.Failure(CompleteOnboardingErrors.AlreadyCompleted());
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<OnboardingProfile>.Success(
                new OnboardingProfile(
                    profile.UserId,
                    profile.DisplayName!,
                    profile.AvatarUrl,
                    profile.OnboardingCompleted));
        }

        private static Error MapValidationError(ArgumentException exception)
        {
            return exception.ParamName switch
            {
                "avatarUrl" => CompleteOnboardingErrors.InvalidAvatarUrl(exception.Message),
                _ => CompleteOnboardingErrors.InvalidDisplayName(exception.Message),
            };
        }
    }
}
