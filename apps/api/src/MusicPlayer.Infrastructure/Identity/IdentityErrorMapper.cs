using Microsoft.AspNetCore.Identity;
using MusicPlayer.Application.Common;
using MusicPlayer.Application.Users.Register;

namespace MusicPlayer.Infrastructure.Identity
{
    internal static class IdentityErrorMapper
    {
        public static Error Map(IEnumerable<IdentityError> errors)
        {
            var identityErrors = errors.ToList();

            if (identityErrors.Count == 0)
            {
                return RegistrationErrors.RegistrationFailed("User creation failed.");
            }

            if (identityErrors.Any(error => error.Code is "DuplicateEmail" or "DuplicateUserName"))
            {
                return RegistrationErrors.EmailAlreadyExists(GetMessage(identityErrors));
            }

            if (identityErrors.Any(error => error.Code == "InvalidEmail"))
            {
                return RegistrationErrors.InvalidEmail(GetMessage(identityErrors));
            }

            if (identityErrors.Any(error => error.Code.StartsWith("Password", StringComparison.Ordinal)))
            {
                return RegistrationErrors.InvalidPassword(GetMessage(identityErrors));
            }

            return RegistrationErrors.RegistrationFailed(GetMessage(identityErrors));
        }

        private static string GetMessage(IReadOnlyList<IdentityError> errors)
        {
            return string.Join(' ', errors.Select(error => error.Description));
        }
    }
}
