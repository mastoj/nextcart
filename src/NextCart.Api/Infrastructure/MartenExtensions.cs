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
        Console.WriteLine("==> Called USING CONNECTION STRING: " + connectionString);
        services.AddMarten(options =>
        {
            options.ConfigureMarten(connectionString);

            // options.Events.InlineProjections.AggregateStreamsWith<Cart>();
            // options.Events.InlineProjections.Add(new CartProjection());
        });
    }

    public static void ConfigureMarten(this StoreOptions options, string connectionString)
    {
        Console.WriteLine("==> USING CONNECTION STRING: " + connectionString);
        options.Connection(connectionString);
        options.UseDefaultSerialization(EnumStorage.AsString, nonPublicMembersStorage: NonPublicMembersStorage.All);
        options.AutoCreateSchemaObjects = AutoCreate.All;
        options.Events.DatabaseSchemaName = Environment.GetEnvironmentVariable("MARTEN_SCHEMANAME")!;
    }

    public static async Task<T> Add<T>(this IDocumentSession documentSession, Guid id, Func<object[]> handle, CancellationToken ct)
        where T : class
    {
        Console.WriteLine("==> Saving: " + id);
        var events = handle();
        events.ToList().ForEach(e => Console.WriteLine("==> Event: " + e));
        documentSession.Events.StartStream<T>(id, events);
        await documentSession.SaveChangesAsync(token: ct);
        return (await documentSession.Events.AggregateStreamAsync<T>(id, token: ct))!;
    }

    public static Task GetAndUpdate<T>(this IDocumentSession documentSession, Guid id, int version,
        Func<T, object[]> handle, CancellationToken ct)
        where T : class =>
        documentSession.Events.WriteToAggregate<T>(id, version, stream =>
            stream.AppendOne(handle(stream.Aggregate)), ct);
}