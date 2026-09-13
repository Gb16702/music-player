using MusicPlayer.Api.Contracts;

namespace MusicPlayer.Api.Validation;

internal static class MagicLinkVerifyRequestValidator
{
    public static Dictionary<string, string[]> Validate(MagicLinkVerifyRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Token))
        {
            errors["token"] = ["Token is required."];
        }

        return errors;
    }
}
