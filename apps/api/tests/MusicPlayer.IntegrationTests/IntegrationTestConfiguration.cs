namespace MusicPlayer.IntegrationTests;

internal static class IntegrationTestConfiguration
{
    public static Dictionary<string, string?> CreateBaseSettings(string? databaseConnectionString = null)
    {
        return new Dictionary<string, string?>
        {
            ["Spotify:ClientId"] = "integration-test-client-id",
            ["Spotify:ClientSecret"] = "integration-test-client-secret",
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
