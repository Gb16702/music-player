using MusicPlayer.Application.Common;

namespace MusicPlayer.Application.Users.GetCurrentUser
{
    public static class GetCurrentUserErrors
    {
        public const string UserNotFoundCode = "user_not_found";

        public static Error UserNotFound() => new(UserNotFoundCode, "User account was not found.");
    }
}
