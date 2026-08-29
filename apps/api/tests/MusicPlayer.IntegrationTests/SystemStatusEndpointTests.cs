using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using MusicPlayer.Api.Contracts;

namespace MusicPlayer.IntegrationTests;

public sealed class SystemStatusEndpointTests(WebApplicationFactory<Program> application)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetSystemStatusReturnsAHealthyPayload()
    {
        using var client = application.CreateClient();

        var response = await client.GetAsync("/api/v1/system/status");

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<SystemStatusResponse>();

        Assert.NotNull(payload);
        Assert.Equal("ok", payload.Status);
        Assert.InRange(payload.Timestamp, DateTimeOffset.UtcNow.AddMinutes(-1), DateTimeOffset.UtcNow.AddMinutes(1));
    }
}
