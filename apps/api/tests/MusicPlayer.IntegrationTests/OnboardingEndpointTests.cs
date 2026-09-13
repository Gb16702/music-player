using System.Net;
using System.Net.Http.Json;
using MusicPlayer.Api.Contracts;

namespace MusicPlayer.IntegrationTests;

public sealed class OnboardingEndpointTests(MusicPlayerApiFactory application)
    : IClassFixture<MusicPlayerApiFactory>
{
    [Fact]
    public async Task CompleteOnboardingReturnsUnauthorizedWhenNotAuthenticated()
    {
        using var client = application.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/onboarding/complete",
            new CompleteOnboardingRequest("Jane Doe", null));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
