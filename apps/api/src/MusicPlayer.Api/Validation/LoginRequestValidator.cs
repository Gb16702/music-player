using System.Net.Mail;
using MusicPlayer.Api.Contracts;

namespace MusicPlayer.Api.Validation;

internal static class LoginRequestValidator
{
    private const int MaxEmailLength = 256;

    public static Dictionary<string, string[]> Validate(LoginRequest request)
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
