namespace MusicPlayer.Application.Abstractions.Identity
{
    public interface IWebAppRedirectBuilder
    {
        string BuildAuthCallbackSuccessUrl();

        string BuildAuthCallbackErrorUrl(string errorCode, string? detail = null);
    }
}
