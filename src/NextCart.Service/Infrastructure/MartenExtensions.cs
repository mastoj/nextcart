namespace NextCart.Service.Infrastructure;

using Marten;
using Microsoft.Extensions.DependencyInjection;
using Weasel.Core;

public static class MartenExtensions
{
    public static void AddMarten(this IServiceCollection services)
    {
        var connectionString = Environment.GetEnvironmentVariable("MARTEN_CONNECTIONSTRING")!;
        Console.WriteLine("==> AddMarten: " + connectionString);
        services.AddMarten(connectionString);
    }

    public static void AddMarten(this IServiceCollection services, string connectionString)
    {
        services.AddMarten(options =>
        {
            options.ConfigureMarten(connectionString);
            // options.Events.InlineProjections.AggregateStreamsWith<Cart>();
            // options.Events.InlineProjections.Add(new CartProjection());
        });
    }

    public static void ConfigureMarten(this StoreOptions options, string connectionString)
    {
        options.Connection(connectionString);
        options.UseDefaultSerialization(EnumStorage.AsString, nonPublicMembersStorage: NonPublicMembersStorage.All);
        options.AutoCreateSchemaObjects = AutoCreate.All;
        options.Events.DatabaseSchemaName = Environment.GetEnvironmentVariable("MARTEN_SCHEMANAME")!;
    }

    public static async Task<T> Add<T>(this IDocumentStore documentStore, Guid id, Func<object[]> handle, CancellationToken ct)
        where T : class
    {
        var events = handle();
        using var documentSession = documentStore.OpenSession();
        documentSession.Events.StartStream<T>(id, events);
        await documentSession.SaveChangesAsync(token: ct);
        return (await documentSession.Events.AggregateStreamAsync<T>(id, token: ct))!;
    }

    public static async Task<T> Update<T>(this IDocumentStore documentStore, Guid id, int version, object[] events, CancellationToken ct)
        where T : class
    {
        using var documentSession = documentStore.OpenSession();
        var expectedVersion = version + events.Length;
        documentSession.Events.Append(id, expectedVersion, events);
        await documentSession.SaveChangesAsync(token: ct);
        return (await documentSession.Events.AggregateStreamAsync<T>(id, token: ct))!;
    }

    public static async Task<T> GetAndUpdate<T>(this IDocumentStore documentStore, Guid id, int version,
        Func<T, object[]> handle, CancellationToken ct)
        where T : class
    {
        using var documentSession = documentStore.OpenSession();
        await documentSession.Events.WriteToAggregate<T>(id, version, stream =>
            stream.AppendMany(handle(stream.Aggregate)), ct);
        await documentSession.SaveChangesAsync(token: ct);
        return (await documentSession.Events.AggregateStreamAsync<T>(id, token: ct))!;
    }
}