namespace MusicPlayer.Domain.Users
{
    public sealed class UserProfile
    {
        public const int MaxDisplayNameLength = 50;

        public UserProfile(Guid userId, string displayName)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }

            UserId = userId;
            DisplayName = NormalizeDisplayName(displayName);
        }

        public Guid UserId { get; private set; }

        public string DisplayName { get; private set; }

        public void ChangeDisplayName(string displayName)
        {
            DisplayName = NormalizeDisplayName(displayName);
        }

        private static string NormalizeDisplayName(string displayName)
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
    }
}
