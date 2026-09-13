namespace MusicPlayer.Api.Contracts;

public sealed record CompleteOnboardingResponse(
    Guid UserId,
    string DisplayName,
    string? AvatarUrl,
    bool OnboardingCompleted);
