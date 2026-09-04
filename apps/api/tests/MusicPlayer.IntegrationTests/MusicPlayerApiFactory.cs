using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace MusicPlayer.IntegrationTests;

public sealed class MusicPlayerApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Spotify:ClientId"] = "integration-test-client-id",
                    ["Spotify:ClientSecret"] = "integration-test-client-secret",
                    ["ConnectionStrings:Database"] = "Host=127.0.0.1;Port=5432;Database=music_player_tests;Username=test;Password=test"
                });
        });
    }
}
