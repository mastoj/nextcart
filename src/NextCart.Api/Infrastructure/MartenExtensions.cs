namespace NextCart.Api.Infrastructure;

using Marten;
using Weasel.Core;

public static class MartenExtensions
{
    public static void AddMarten(this IServiceCollection services)
    {
        var connectionString = Environment.GetEnvironmentVariable("MARTEN_CONNECTIONSTRING")!;
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

    public static async Task<T> Add<T>(this IDocumentStore documentStore, string id, Func<object[]> handle, CancellationToken ct)
        where T : class
    {
        var events = handle();
        using var documentSession = documentStore.LightweightSession();
        var guidId = Guid.Parse(id);
        documentSession.Events.StartStream<T>(guidId, events);
        await documentSession.SaveChangesAsync(token: ct);
        return (await documentSession.Events.AggregateStreamAsync<T>(guidId, token: ct))!;
    }

    public static async Task<T> GetAndUpdate<T>(this IDocumentStore documentStore, Guid id, int version,
        Func<T, object[]> handle, CancellationToken ct)
        where T : class
    {
        using var documentSession = documentStore.LightweightSession();
        await documentSession.Events.WriteToAggregate<T>(id, version, stream =>
            stream.AppendMany(handle(stream.Aggregate)), ct);
        return (await documentSession.Events.AggregateStreamAsync<T>(id, token: ct))!;
    }
}