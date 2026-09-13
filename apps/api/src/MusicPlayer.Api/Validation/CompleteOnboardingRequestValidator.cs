using MusicPlayer.Api.Contracts;
using MusicPlayer.Domain.Users;

namespace MusicPlayer.Api.Validation;

internal static class CompleteOnboardingRequestValidator
{
    public static Dictionary<string, string[]> Validate(CompleteOnboardingRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            errors["displayName"] = ["Display name is required."];
        }
        else if (request.DisplayName.Trim().Length > UserProfile.MaxDisplayNameLength)
        {
            errors["displayName"] = [$"Display name cannot exceed {UserProfile.MaxDisplayNameLength} characters."];
        }

        if (request.AvatarUrl is not null
            && !string.IsNullOrWhiteSpace(request.AvatarUrl)
            && request.AvatarUrl.Trim().Length > UserProfile.MaxAvatarUrlLength)
        {
            errors["avatarUrl"] = [$"Avatar URL cannot exceed {UserProfile.MaxAvatarUrlLength} characters."];
        }

        return errors;
    }
}
