using Testcontainers.PostgreSql;

namespace MusicPlayer.IntegrationTests;

public sealed class PostgresTestFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _container;

    public bool IsAvailable { get; private set; }

    public string ConnectionString { get; private set; } = string.Empty;

    public async Task InitializeAsync()
    {
        try
        {
            _container = new PostgreSqlBuilder()
                .WithImage("postgres:18-alpine")
                .Build();

            await _container.StartAsync();

            ConnectionString = _container.GetConnectionString();
            IsAvailable = true;
        }
        catch (Exception)
        {
            IsAvailable = false;
        }
    }

    public async Task DisposeAsync()
    {
        if (_container is not null)
        {
            await _container.DisposeAsync();
        }
    }
}

[CollectionDefinition(nameof(PostgresTestCollection))]
public sealed class PostgresTestCollection : ICollectionFixture<PostgresTestFixture>;
