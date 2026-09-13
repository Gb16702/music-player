namespace MusicPlayer.Application.Abstractions.Identity
{
    public interface IMagicLinkUrlBuilder
    {
        string BuildVerifyUrl(string token);
    }
}
