using Microsoft.AspNetCore.WebUtilities;

namespace MusicPlayer.IntegrationTests;

internal static class MagicLinkUrlParser
{
    public static string? ExtractToken(string magicLinkUrl)
    {
        var uri = new Uri(magicLinkUrl);
        var query = QueryHelpers.ParseQuery(uri.Query);

        return query.TryGetValue("token", out var token) ? token.ToString() : null;
    }
}
