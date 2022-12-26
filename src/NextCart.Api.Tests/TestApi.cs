using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using NextCart.Api.Infrastructure;

namespace NextCart.Api.Tests;

public class PostgreSQLFixture : IAsyncLifetime
{
    private readonly TestcontainerDatabase testcontainers = new TestcontainersBuilder<PostgreSqlTestcontainer>()
        .WithDatabase(new PostgreSqlTestcontainerConfiguration()
        {
            Database = "test_db",
            Username = "postgres",
            Password = "postgres",
        })
        .WithImage("postgres:11")
        .Build();

    public string? ConnectionString { get; private set; }

    public PostgreSQLFixture()
    {

    }

    public async Task InitializeAsync()
    {
        await testcontainers.StartAsync();
        Console.WriteLine("==> INITIALIZED DB: " + testcontainers.ConnectionString);
        Environment.SetEnvironmentVariable("MARTEN_SCHEMANAME", "cart");
        var ds = Marten.DocumentStore.For(options => options.ConfigureMarten(testcontainers.ConnectionString));
        ConnectionString = testcontainers.ConnectionString;
    }

    public Task DisposeAsync()
    {
        Console.WriteLine("==> CLOSING DB");
        return testcontainers.DisposeAsync().AsTask();
    }
}

// [CollectionDefinition("Postgres collection")]
// public class DatabaseCollection : ICollectionFixture<PostgreSQLFixture>
// {
//     // This class has no code, and is never created. Its purpose is simply
//     // to be the place to apply [CollectionDefinition] and all the
//     // ICollectionFixture<> interfaces.
// }

public class TestApi : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public TestApi(PostgreSQLFixture fixture)
    {
        Console.WriteLine("==> INITIALIZING API: " + fixture.ConnectionString);
        Environment.SetEnvironmentVariable("MARTEN_SCHEMANAME", "cart");
        _connectionString = fixture.ConnectionString!;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddMarten(_connectionString);
        });
        // base.ConfigureWebHost(builder);
    }
}
