using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MusicPlayer.Api.Contracts;
using MusicPlayer.Application.Abstractions.Email;
using MusicPlayer.Infrastructure.Persistence;
using Xunit;

namespace MusicPlayer.IntegrationTests;

[Collection(nameof(PostgresTestCollection))]
public sealed class GoogleAuthCallbackDatabaseTests(PostgresTestFixture postgres)
{
    [SkippableFact]
    public async Task GoogleCallbackCreatesSessionForNewUser()
    {
        Skip.IfNot(postgres.IsAvailable, "Docker is required for database integration tests.");

        await using var factory = CreateFactory();
        await MigrateDatabaseAsync(factory);

        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            HandleCookies = true,
            AllowAutoRedirect = false
        });

        var email = $"google-{Guid.NewGuid():N}@example.com";
        var providerKey = $"google-subject-{Guid.NewGuid():N}";

        var seedResponse = await client.GetAsync(
            $"/integration-test/seed-google-external?email={Uri.EscapeDataString(email)}&key={Uri.EscapeDataString(providerKey)}");

        Assert.Equal(HttpStatusCode.OK, seedResponse.StatusCode);

        var callbackResponse = await client.GetAsync("/api/v1/auth/google/callback");

        Assert.Equal(HttpStatusCode.Redirect, callbackResponse.StatusCode);

        var location = callbackResponse.Headers.Location?.ToString();

        Assert.NotNull(location);
        Assert.Contains("/auth/callback", location, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("error=", location, StringComparison.OrdinalIgnoreCase);

        var meResponse = await client.GetAsync("/api/v1/auth/me");

        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);

        var currentUser = await meResponse.Content.ReadFromJsonAsync<CurrentUserResponse>();

        Assert.NotNull(currentUser);
        Assert.Equal(email, currentUser.Email);
        Assert.False(currentUser.OnboardingCompleted);
    }

    private WebApplicationFactory<Program> CreateFactory()
    {
        return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            IntegrationTestConfiguration.ConfigureTestHost(builder, postgres.ConnectionString);

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IEmailSender>();
                services.AddSingleton<CapturingEmailSender>();
                services.AddSingleton<IEmailSender>(provider => provider.GetRequiredService<CapturingEmailSender>());
                services.AddSingleton<IStartupFilter, GoogleExternalLoginTestStartupFilter>();
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
