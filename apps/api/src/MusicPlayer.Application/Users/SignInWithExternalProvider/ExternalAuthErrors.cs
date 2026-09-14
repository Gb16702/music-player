using MusicPlayer.Application.Common;

namespace MusicPlayer.Application.Users.SignInWithExternalProvider
{
    public static class ExternalAuthErrors
    {
        public const string EmailRequiredCode = "external_auth_email_required";

        public const string SignInFailedCode = "external_auth_sign_in_failed";

        public static Error EmailRequired() => new(EmailRequiredCode, "External provider did not return an email address.");

        public static Error SignInFailed(string message) => new(SignInFailedCode, message);
    }
}
