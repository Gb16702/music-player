namespace MusicPlayer.Infrastructure.Email
{
    internal sealed class ResendOptions
    {
        public const string SectionName = "Resend";

        public string ApiKey { get; set; } = string.Empty;
    }
}
