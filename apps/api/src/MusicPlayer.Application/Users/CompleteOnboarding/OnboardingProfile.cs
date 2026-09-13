namespace MusicPlayer.Application.Users.CompleteOnboarding
{
    public sealed record OnboardingProfile(
        Guid UserId,
        string DisplayName,
        string? AvatarUrl,
        bool OnboardingCompleted);
}
