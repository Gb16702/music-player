using System.Net;

namespace MusicPlayer.IntegrationTests;

public sealed class GoogleAuthEndpointTests(MusicPlayerApiFactory application)
    : IClassFixture<MusicPlayerApiFactory>
{
    [Fact]
    public async Task LoginWithGoogleRedirectsToGoogle()
    {
        using var client = application.CreateClient(new()
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/api/v1/auth/google/login");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        var location = response.Headers.Location?.ToString();

        Assert.NotNull(location);
        Assert.Contains("accounts.google.com", location, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(
            "signin-google",
            Uri.UnescapeDataString(location),
            StringComparison.OrdinalIgnoreCase);
    }
}
