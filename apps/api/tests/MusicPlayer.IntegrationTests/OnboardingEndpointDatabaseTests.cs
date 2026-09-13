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
public sealed class OnboardingEndpointDatabaseTests(PostgresTestFixture postgres)
{
    [SkippableFact]
    public async Task CompleteOnboardingSetsProfileAndReturnsCompletedState()
    {
        Skip.IfNot(postgres.IsAvailable, "Docker is required for database integration tests.");

        await using var factory = CreateFactory();
        await MigrateDatabaseAsync(factory);

        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true });

        var email = $"onboarding-{Guid.NewGuid():N}@example.com";
        var password = "Password1!";
        var displayName = "Jane Doe";
        var avatarUrl = "https://example.com/avatar.png";

        await client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest(email, password));
        await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(email, password, true));

        var response = await client.PostAsJsonAsync(
            "/api/v1/onboarding/complete",
            new CompleteOnboardingRequest(displayName, avatarUrl));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<CompleteOnboardingResponse>();

        Assert.NotNull(payload);
        Assert.Equal(displayName, payload.DisplayName);
        Assert.Equal(avatarUrl, payload.AvatarUrl);
        Assert.True(payload.OnboardingCompleted);

        var meResponse = await client.GetAsync("/api/v1/auth/me");
        var currentUser = await meResponse.Content.ReadFromJsonAsync<CurrentUserResponse>();

        Assert.NotNull(currentUser);
        Assert.Equal(displayName, currentUser.DisplayName);
        Assert.Equal(avatarUrl, currentUser.AvatarUrl);
        Assert.True(currentUser.OnboardingCompleted);
    }

    [SkippableFact]
    public async Task CompleteOnboardingReturnsConflictWhenAlreadyCompleted()
    {
        Skip.IfNot(postgres.IsAvailable, "Docker is required for database integration tests.");

        await using var factory = CreateFactory();
        await MigrateDatabaseAsync(factory);

        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true });

        var email = $"onboarding-conflict-{Guid.NewGuid():N}@example.com";
        var password = "Password1!";

        await client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest(email, password));
        await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(email, password, true));

        var firstResponse = await client.PostAsJsonAsync(
            "/api/v1/onboarding/complete",
            new CompleteOnboardingRequest("Jane Doe", null));

        var secondResponse = await client.PostAsJsonAsync(
            "/api/v1/onboarding/complete",
            new CompleteOnboardingRequest("Another Name", null));

        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);

        var error = await secondResponse.Content.ReadFromJsonAsync<AuthErrorResponse>();

        Assert.NotNull(error);
        Assert.Equal("onboarding_already_completed", error.Code);
    }

    private WebApplicationFactory<Program> CreateFactory()
    {
        return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddInMemoryCollection(IntegrationTestConfiguration.CreateBaseSettings(postgres.ConnectionString));
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
