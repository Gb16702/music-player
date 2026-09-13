using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicPlayer.Api.Contracts;
using MusicPlayer.Infrastructure.Persistence;
using Xunit;

namespace MusicPlayer.IntegrationTests;

[Collection(nameof(PostgresTestCollection))]
public sealed class AuthSessionEndpointDatabaseTests(PostgresTestFixture postgres)
{
    [SkippableFact]
    public async Task LogoutReturnsNoContentAndClearsSession()
    {
        Skip.IfNot(postgres.IsAvailable, "Docker is required for database integration tests.");

        await using var factory = CreateFactory();
        await MigrateDatabaseAsync(factory);

        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true });

        var email = $"logout-{Guid.NewGuid():N}@example.com";
        var password = "Password1!";

        var registerResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest(email, password, "Jane Doe"));

        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        var loginResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest(email, password, true));

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var meBeforeLogout = await client.GetAsync("/api/v1/auth/me");

        Assert.Equal(HttpStatusCode.OK, meBeforeLogout.StatusCode);

        var logoutResponse = await client.PostAsync("/api/v1/auth/logout", null);

        Assert.Equal(HttpStatusCode.NoContent, logoutResponse.StatusCode);

        var meAfterLogout = await client.GetAsync("/api/v1/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, meAfterLogout.StatusCode);
    }

    [SkippableFact]
    public async Task GetCurrentUserReturnsProfileWhenAuthenticated()
    {
        Skip.IfNot(postgres.IsAvailable, "Docker is required for database integration tests.");

        await using var factory = CreateFactory();
        await MigrateDatabaseAsync(factory);

        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true });

        var email = $"me-{Guid.NewGuid():N}@example.com";
        var password = "Password1!";
        var displayName = "Jane Doe";

        var registerResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest(email, password, displayName));

        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        var loginResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest(email, password, true));

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var meResponse = await client.GetAsync("/api/v1/auth/me");

        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);

        var payload = await meResponse.Content.ReadFromJsonAsync<CurrentUserResponse>();

        Assert.NotNull(payload);
        Assert.NotEqual(Guid.Empty, payload.UserId);
        Assert.Equal(email, payload.Email);
        Assert.Equal(displayName, payload.DisplayName);
    }

    private WebApplicationFactory<Program> CreateFactory()
    {
        return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:Database"] = postgres.ConnectionString,
                        ["Spotify:ClientId"] = "integration-test-client-id",
                        ["Spotify:ClientSecret"] = "integration-test-client-secret"
                    });
            });
        });
    }

    private static async Task MigrateDatabaseAsync(WebApplicationFactory<Program> factory)
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MusicPlayerDbContext>();

        await dbContext.Database.MigrateAsync();
    }
}
