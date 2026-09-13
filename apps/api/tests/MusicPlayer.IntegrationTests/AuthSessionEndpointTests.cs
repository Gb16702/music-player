using System.Net;

namespace MusicPlayer.IntegrationTests;

public sealed class AuthSessionEndpointTests(MusicPlayerApiFactory application)
    : IClassFixture<MusicPlayerApiFactory>
{
    [Fact]
    public async Task GetCurrentUserReturnsUnauthorizedWhenNotAuthenticated()
    {
        using var client = application.CreateClient();

        var response = await client.GetAsync("/api/v1/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task LogoutReturnsNoContentWhenNotAuthenticated()
    {
        using var client = application.CreateClient();

        var response = await client.PostAsync("/api/v1/auth/logout", null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
