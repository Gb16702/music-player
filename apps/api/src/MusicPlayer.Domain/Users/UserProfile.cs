namespace MusicPlayer.Domain.Users
{
    public sealed class UserProfile
    {
        public const int MaxDisplayNameLength = 50;

        public const int MaxAvatarUrlLength = 2048;

        public UserProfile(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }

            UserId = userId;
            OnboardingCompleted = false;
        }

        public Guid UserId { get; private set; }

        public string? DisplayName { get; private set; }

        public string? AvatarUrl { get; private set; }

        public bool OnboardingCompleted { get; private set; }

        public void CompleteOnboarding(string displayName, string? avatarUrl)
        {
            if (OnboardingCompleted)
            {
                throw new InvalidOperationException("Onboarding has already been completed.");
            }

            DisplayName = NormalizeDisplayName(displayName);
            AvatarUrl = NormalizeAvatarUrl(avatarUrl);
            OnboardingCompleted = true;
        }

        public void ChangeDisplayName(string displayName)
        {
            DisplayName = NormalizeDisplayName(displayName);
        }

        public static string NormalizeDisplayName(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Display name cannot be null or whitespace.", nameof(displayName));
            }

            var normalizedDisplayName = displayName.Trim();

            if (normalizedDisplayName.Length > MaxDisplayNameLength)
            {
                throw new ArgumentException($"Display name cannot exceed {MaxDisplayNameLength} characters.", nameof(displayName));
            }

            return normalizedDisplayName;
        }

        public static string? NormalizeAvatarUrl(string? avatarUrl)
        {
            if (string.IsNullOrWhiteSpace(avatarUrl))
            {
                return null;
            }

            var normalizedAvatarUrl = avatarUrl.Trim();

            if (normalizedAvatarUrl.Length > MaxAvatarUrlLength)
            {
                throw new ArgumentException($"Avatar URL cannot exceed {MaxAvatarUrlLength} characters.", nameof(avatarUrl));
            }

            if (!Uri.TryCreate(normalizedAvatarUrl, UriKind.Absolute, out var uri)
                || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException("Avatar URL must be a valid absolute HTTP or HTTPS URL.", nameof(avatarUrl));
            }

            return normalizedAvatarUrl;
        }
    }
}
