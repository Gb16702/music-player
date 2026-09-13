using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MusicPlayer.Api.Contracts;

namespace MusicPlayer.IntegrationTests;

public sealed class MagicLinkEndpointTests(MusicPlayerApiFactory application)
    : IClassFixture<MusicPlayerApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task RequestMagicLinkReturnsNoContentForValidEmail()
    {
        using var client = application.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/auth/magic-link/request",
            new MagicLinkRequest("user@example.com"));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task RequestMagicLinkReturnsValidationProblemWhenEmailIsMissing()
    {
        using var client = application.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/auth/magic-link/request",
            new MagicLinkRequest(string.Empty));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(JsonOptions);

        Assert.NotNull(problem);
        Assert.True(problem.Errors.ContainsKey("email"));
    }

    [Fact]
    public async Task VerifyMagicLinkReturnsBadRequestWhenTokenIsMissing()
    {
        using var client = application.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/auth/magic-link/verify",
            new MagicLinkVerifyRequest(string.Empty));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(JsonOptions);

        Assert.NotNull(problem);
        Assert.True(problem.Errors.ContainsKey("token"));
    }

    private sealed class ValidationProblemDetails
    {
        public Dictionary<string, string[]> Errors { get; set; } = [];
    }
}
