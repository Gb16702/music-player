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
public sealed class RegisterEndpointDatabaseTests(PostgresTestFixture postgres)
{
    [SkippableFact]
    public async Task RegisterCreatesUserAndPendingProfile()
    {
        Skip.IfNot(postgres.IsAvailable, "Docker is required for database integration tests.");

        await using var factory = CreateFactory();
        await MigrateDatabaseAsync(factory);

        using var client = factory.CreateClient();
        var email = $"user-{Guid.NewGuid():N}@example.com";

        var response = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest(email, "Password1!"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<RegisterResponse>();

        Assert.NotNull(payload);
        Assert.NotEqual(Guid.Empty, payload.UserId);

        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MusicPlayerDbContext>();
        var profile = await dbContext.UserProfiles.SingleAsync(item => item.UserId == payload.UserId);

        Assert.Null(profile.DisplayName);
        Assert.False(profile.OnboardingCompleted);
    }

    [SkippableFact]
    public async Task RegisterReturnsConflictWhenEmailAlreadyExists()
    {
        Skip.IfNot(postgres.IsAvailable, "Docker is required for database integration tests.");

        await using var factory = CreateFactory();
        await MigrateDatabaseAsync(factory);

        using var client = factory.CreateClient();
        var email = $"duplicate-{Guid.NewGuid():N}@example.com";
        var request = new RegisterRequest(email, "Password1!");

        var firstResponse = await client.PostAsJsonAsync("/api/v1/auth/register", request);
        var secondResponse = await client.PostAsJsonAsync("/api/v1/auth/register", request);

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);

        var error = await secondResponse.Content.ReadFromJsonAsync<AuthErrorResponse>();

        Assert.NotNull(error);
        Assert.Equal("email_already_exists", error.Code);
    }

    [SkippableFact]
    public async Task LoginReturnsCookieAfterRegistration()
    {
        Skip.IfNot(postgres.IsAvailable, "Docker is required for database integration tests.");

        await using var factory = CreateFactory();
        await MigrateDatabaseAsync(factory);

        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true });

        var email = $"login-{Guid.NewGuid():N}@example.com";
        var password = "Password1!";

        var registerResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest(email, password));

        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        var loginResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest(email, password, true));

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.NotNull(loginResponse.Headers.GetValues("Set-Cookie").FirstOrDefault());
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
