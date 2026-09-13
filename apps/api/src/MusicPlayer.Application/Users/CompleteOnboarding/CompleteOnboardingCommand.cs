namespace MusicPlayer.Application.Users.CompleteOnboarding
{
    public sealed record CompleteOnboardingCommand(Guid UserId, string DisplayName, string? AvatarUrl);
}
