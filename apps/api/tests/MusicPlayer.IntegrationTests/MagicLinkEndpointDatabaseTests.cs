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
public sealed class MagicLinkEndpointDatabaseTests(PostgresTestFixture postgres)
{
    [SkippableFact]
    public async Task VerifyMagicLinkCreatesSessionForNewUser()
    {
        Skip.IfNot(postgres.IsAvailable, "Docker is required for database integration tests.");

        var emailSender = new CapturingEmailSender();
        await using var factory = CreateFactory(emailSender);
        await MigrateDatabaseAsync(factory);

        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true });
        var email = $"magic-link-{Guid.NewGuid():N}@example.com";

        var requestResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/magic-link/request",
            new MagicLinkRequest(email));

        Assert.Equal(HttpStatusCode.NoContent, requestResponse.StatusCode);
        Assert.Equal(email, emailSender.LastRecipient);

        var token = MagicLinkUrlParser.ExtractToken(emailSender.LastMagicLinkUrl!);

        Assert.False(string.IsNullOrWhiteSpace(token));

        var verifyResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/magic-link/verify",
            new MagicLinkVerifyRequest(token!));

        Assert.Equal(HttpStatusCode.OK, verifyResponse.StatusCode);
        Assert.NotNull(verifyResponse.Headers.GetValues("Set-Cookie").FirstOrDefault());

        var meResponse = await client.GetAsync("/api/v1/auth/me");

        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);

        var currentUser = await meResponse.Content.ReadFromJsonAsync<CurrentUserResponse>();

        Assert.NotNull(currentUser);
        Assert.Equal(email, currentUser.Email);
        Assert.False(currentUser.OnboardingCompleted);
    }

    [SkippableFact]
    public async Task VerifyMagicLinkSignsInExistingPasswordUser()
    {
        Skip.IfNot(postgres.IsAvailable, "Docker is required for database integration tests.");

        var emailSender = new CapturingEmailSender();
        await using var factory = CreateFactory(emailSender);
        await MigrateDatabaseAsync(factory);

        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true });
        var email = $"magic-link-existing-{Guid.NewGuid():N}@example.com";
        var password = "Password1!";

        var registerResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest(email, password));

        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        emailSender.Reset();

        var requestResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/magic-link/request",
            new MagicLinkRequest(email));

        Assert.Equal(HttpStatusCode.NoContent, requestResponse.StatusCode);

        var token = MagicLinkUrlParser.ExtractToken(emailSender.LastMagicLinkUrl!);

        Assert.False(string.IsNullOrWhiteSpace(token));

        using var magicLinkClient = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true });

        var verifyResponse = await magicLinkClient.PostAsJsonAsync(
            "/api/v1/auth/magic-link/verify",
            new MagicLinkVerifyRequest(token!));

        Assert.Equal(HttpStatusCode.OK, verifyResponse.StatusCode);

        var meResponse = await magicLinkClient.GetAsync("/api/v1/auth/me");

        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);

        var currentUser = await meResponse.Content.ReadFromJsonAsync<CurrentUserResponse>();

        Assert.NotNull(currentUser);
        Assert.Equal(email, currentUser.Email);
    }

    private WebApplicationFactory<Program> CreateFactory(CapturingEmailSender emailSender)
    {
        return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddInMemoryCollection(IntegrationTestConfiguration.CreateBaseSettings(postgres.ConnectionString));
            });

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IEmailSender>();
                services.AddSingleton(emailSender);
                services.AddSingleton<IEmailSender>(provider => provider.GetRequiredService<CapturingEmailSender>());
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
