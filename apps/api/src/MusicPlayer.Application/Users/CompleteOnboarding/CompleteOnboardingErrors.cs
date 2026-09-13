using MusicPlayer.Application.Common;

namespace MusicPlayer.Application.Users.CompleteOnboarding
{
    public static class CompleteOnboardingErrors
    {
        public const string ProfileNotFoundCode = "profile_not_found";

        public const string AlreadyCompletedCode = "onboarding_already_completed";

        public const string InvalidDisplayNameCode = "invalid_display_name";

        public const string InvalidAvatarUrlCode = "invalid_avatar_url";

        public static Error ProfileNotFound() => new(ProfileNotFoundCode, "User profile was not found.");

        public static Error AlreadyCompleted() => new(AlreadyCompletedCode, "Onboarding has already been completed.");

        public static Error InvalidDisplayName(string message) => new(InvalidDisplayNameCode, message);

        public static Error InvalidAvatarUrl(string message) => new(InvalidAvatarUrlCode, message);
    }
}
