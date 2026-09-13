namespace MusicPlayer.Application.Users.GetCurrentUser
{
    public sealed record CurrentUser(Guid UserId, string Email, string DisplayName);
}
