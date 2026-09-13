namespace MusicPlayer.Infrastructure.Email
{
    internal sealed class MagicLinkOptions
    {
        public const string SectionName = "MagicLink";

        public string WebAppBaseUrl { get; set; } = string.Empty;

        public int TokenLifetimeMinutes { get; set; } = 15;
    }
}
