using System.Net;

namespace MusicPlayer.IntegrationTests;

public sealed class SpotifyAuthEndpointTests(MusicPlayerApiFactory application)
    : IClassFixture<MusicPlayerApiFactory>
{
    [Fact]
    public async Task LoginWithSpotifyRedirectsToSpotify()
    {
        using var client = application.CreateClient(new()
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/api/v1/auth/spotify/login");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        var location = response.Headers.Location?.ToString();

        Assert.NotNull(location);
        Assert.Contains("accounts.spotify.com", location, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(
            "signin-spotify",
            Uri.UnescapeDataString(location),
            StringComparison.OrdinalIgnoreCase);
    }
}
