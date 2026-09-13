namespace MusicPlayer.Infrastructure.Email
{
    internal sealed class EmailOptions
    {
        public const string SectionName = "Email";

        public string Provider { get; set; } = "Log";

        public string FromAddress { get; set; } = string.Empty;

        public string FromName { get; set; } = "Music Player";
    }
}
