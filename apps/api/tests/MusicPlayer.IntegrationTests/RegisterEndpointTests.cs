using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MusicPlayer.Api.Contracts;

namespace MusicPlayer.IntegrationTests;

public sealed class RegisterEndpointTests(MusicPlayerApiFactory application)
    : IClassFixture<MusicPlayerApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task RegisterReturnsValidationProblemWhenEmailIsMissing()
    {
        using var client = application.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest(string.Empty, "Password1!"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(JsonOptions);

        Assert.NotNull(problem);
        Assert.True(problem.Errors.ContainsKey("email"));
    }

    [Fact]
    public async Task RegisterReturnsValidationProblemWhenPasswordIsMissing()
    {
        using var client = application.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest("user@example.com", string.Empty));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(JsonOptions);

        Assert.NotNull(problem);
        Assert.True(problem.Errors.ContainsKey("password"));
    }

    private sealed class ValidationProblemDetails
    {
        public Dictionary<string, string[]> Errors { get; set; } = [];
    }
}
