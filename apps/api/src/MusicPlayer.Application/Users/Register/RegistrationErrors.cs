using MusicPlayer.Application.Common;

namespace MusicPlayer.Application.Users.Register
{
    public static class RegistrationErrors
    {
        public const string EmailAlreadyExistsCode = "email_already_exists";
        public const string InvalidEmailCode = "invalid_email";
        public const string InvalidPasswordCode = "invalid_password";
        public const string RegistrationFailedCode = "registration_failed";

        public static Error EmailAlreadyExists(string message) => new(EmailAlreadyExistsCode, message);

        public static Error InvalidEmail(string message) => new(InvalidEmailCode, message);

        public static Error InvalidPassword(string message) => new(InvalidPasswordCode, message);

        public static Error RegistrationFailed(string message) => new(RegistrationFailedCode, message);
    }
}
