using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PgScalabilityTest.Data;
using Testcontainers.PostgreSql;

namespace PgScalabilityTest.Services;

public class TestServerBuilder
{
    private TestServerBuilder()
    {
        Services = new ServiceCollection();
        PostgresSqlBuilder = new PostgreSqlBuilder();
    }

    public IServiceCollection Services { get; }
    private PostgreSqlBuilder PostgresSqlBuilder { get; }

    public static TestServerBuilder CreateDefaultBuilder(string databaseName = "AnalyticsDb")
    {
        var builder = new TestServerBuilder();

        builder.PostgresSqlBuilder
            .WithDatabase(databaseName);

        builder.Services.AddCoreDataServices();

        return builder;
    }

    public TestServer Build() => new(Services.BuildServiceProvider(true), PostgresSqlBuilder.Build());
}

public class TestServer : IAsyncDisposable
{
    private PostgreSqlContainer _dbContainer;

    internal TestServer(IServiceProvider serviceProvider, PostgreSqlContainer dbContainer)
    {
        ServiceProvider = serviceProvider;
        _dbContainer = dbContainer;
    }

    public IServiceProvider ServiceProvider { get; }

    public async Task StartAsync(CancellationToken token = default)
    {
        await _dbContainer.StartAsync(token).ConfigureAwait(false);

        // Set the connection string in the configuration
        var dbSettings = ServiceProvider.GetRequiredService<IOptions<DbSettings>>().Value;
        dbSettings.ConnectionString = _dbContainer.GetConnectionString();

        await EnsureDatabaseCreatedAsync(token);
    }

    private async Task EnsureDatabaseCreatedAsync(CancellationToken token = default)
    {
        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AnalyticsDbContext>();
        await context.Database.EnsureCreatedAsync(token).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}
