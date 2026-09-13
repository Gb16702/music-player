using MusicPlayer.Application.Common;

namespace MusicPlayer.Application.Users.CompleteOnboarding
{
    public interface ICompleteOnboardingHandler
    {
        Task<Result<OnboardingProfile>> HandleAsync(CompleteOnboardingCommand command, CancellationToken cancellationToken);
    }
}
