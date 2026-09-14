namespace MusicPlayer.Application.Users.SignInWithExternalProvider
{
    public sealed record SignInWithExternalProviderCommand(string Email, string LoginProvider, string ProviderKey);
}
