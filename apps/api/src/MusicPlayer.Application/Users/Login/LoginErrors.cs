using MusicPlayer.Application.Common;

namespace MusicPlayer.Application.Users.Login
{
    public static class LoginErrors
    {
        public const string InvalidCredentialsCode = "invalid_credentials";

        public static Error InvalidCredentials() => new(InvalidCredentialsCode, "Invalid email or password.");
    }
}
