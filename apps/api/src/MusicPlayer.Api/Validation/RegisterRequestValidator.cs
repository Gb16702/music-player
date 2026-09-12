using System.Net.Mail;
using MusicPlayer.Api.Contracts;
using MusicPlayer.Domain.Users;

namespace MusicPlayer.Api.Validation;

internal static class RegisterRequestValidator
{
    private const int MaxEmailLength = 256;

    public static Dictionary<string, string[]> Validate(RegisterRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            errors["email"] = ["Email is required."];
        }
        else
        {
            var email = request.Email.Trim();

            if (email.Length > MaxEmailLength)
            {
                errors["email"] = [$"Email cannot exceed {MaxEmailLength} characters."];
            }
            else if (!IsValidEmail(email))
            {
                errors["email"] = ["Email format is invalid."];
            }
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            errors["password"] = ["Password is required."];
        }

        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            errors["displayName"] = ["Display name is required."];
        }
        else if (request.DisplayName.Trim().Length > UserProfile.MaxDisplayNameLength)
        {
            errors["displayName"] = [$"Display name cannot exceed {UserProfile.MaxDisplayNameLength} characters."];
        }

        return errors;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            _ = new MailAddress(email);

            return email.Contains('@', StringComparison.Ordinal);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
