namespace MusicPlayer.Infrastructure.Email
{
    internal sealed class SmtpOptions
    {
        public const string SectionName = "Smtp";

        public string Host { get; set; } = "127.0.0.1";

        public int Port { get; set; } = 1025;
    }
}
