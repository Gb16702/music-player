using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace MusicPlayer.IntegrationTests;

internal static class IntegrationTestConfiguration
{
    public static void ConfigureTestHost(IWebHostBuilder builder, string? databaseConnectionString = null)
    {
        var settings = CreateBaseSettings(databaseConnectionString);

        builder.UseEnvironment("IntegrationTests");

        if (settings.TryGetValue("ConnectionStrings:Database", out var connectionString)
            && !string.IsNullOrWhiteSpace(connectionString))
        {
            builder.UseSetting("ConnectionStrings:Database", connectionString);
        }

        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.Sources.Clear();
            configuration.AddInMemoryCollection(settings);
        });
    }

    public static Dictionary<string, string?> CreateBaseSettings(string? databaseConnectionString = null)
    {
        return new Dictionary<string, string?>
        {
            ["Spotify:ClientId"] = "integration-test-client-id",
            ["Spotify:ClientSecret"] = "integration-test-client-secret",
            ["Google:ClientId"] = "integration-test-google-client-id",
            ["Google:ClientSecret"] = "integration-test-google-client-secret",
            ["Email:Provider"] = "Log",
            ["Email:FromAddress"] = "noreply@music-player.test",
            ["Email:FromName"] = "Music Player",
            ["MagicLink:WebAppBaseUrl"] = "http://localhost:3000",
            ["MagicLink:TokenLifetimeMinutes"] = "15",
            ["ConnectionStrings:Database"] = databaseConnectionString
                ?? "Host=127.0.0.1;Port=5432;Database=music_player_tests;Username=test;Password=test"
        };
    }
}
