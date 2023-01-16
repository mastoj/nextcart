using NextCart.Contracts.Cart.Proto;
using NextCart.Domain.Cart;
using Proto;
using Proto.Cluster;
using Proto.Cluster.Kubernetes;
using Proto.Cluster.Partition;
using Proto.Cluster.Seed;
using Proto.Cluster.Testing;
using Proto.DependencyInjection;
using Proto.Remote;
using Proto.Remote.GrpcNet;

namespace NextCart.Api.Infrastructure;

public static class ProtoActorExtensions
{
    public static void AddTestActorSystem(this IServiceCollection serviceCollection)
    {
        var remoteConfig = GrpcNetRemoteConfig
            .BindToLocalhost();

        IClusterProvider clusterProvider = new TestProvider(new TestProviderOptions(), new InMemAgent());

        (string kind, Props props)[] clusterKinds(IServiceProvider provider) => new[]
        {
            (kind: CartGrainActor.Kind, props: Props.FromProducer(() =>
                new CartGrainActor((context, clusterIdentity) =>
                    ActivatorUtilities.CreateInstance<CartGrain>(provider, context))))
        };
        serviceCollection.AddActorSystem(clusterProvider, remoteConfig, clusterKinds);
    }

    public static void AddKubernetesActorSystem(this IServiceCollection serviceCollection, string advertisedHost)
    {
        var remoteConfig = GrpcNetRemoteConfig
            .BindToAllInterfaces(advertisedHost: advertisedHost)
            .WithProtoMessages(CartMessagesReflection.Descriptor);

        IClusterProvider clusterProvider = new KubernetesProvider();
        serviceCollection.AddActorSystem(clusterProvider, remoteConfig);
    }

    public static void AddDockerActorSystem(this IServiceCollection serviceCollection, string seedHost)
    {
        var seedPort = 8090;
        var remoteConfig =
                    GrpcNetRemoteConfig
                        .BindToAllInterfaces(advertisedHost: seedHost, port: seedPort)
                        .WithProtoMessages(CartMessagesReflection.Descriptor);

        IClusterProvider clusterProvider = new SeedNodeClusterProvider(new SeedNodeClusterProviderOptions((seedHost, seedPort)));
        serviceCollection.AddActorSystem(clusterProvider, remoteConfig);
    }

    private static void AddActorSystem(this IServiceCollection serviceCollection,
        IClusterProvider clusterProvider,
        GrpcNetRemoteConfig remoteConfig,
        Func<IServiceProvider, (string kind, Props props)[]>? clusterKinds = null)
    {
        var actualClusterKinds = clusterKinds is null ? _ => Array.Empty<(string kind, Props props)>() : clusterKinds;
        serviceCollection.AddSingleton(provider =>
        {
            // actor system configuration

            var actorSystemConfig = ActorSystemConfig
                .Setup();

            // cluster configuration
            var clusterConfig = ClusterConfig
                .Setup(
                    clusterName: "NextCart",
                    clusterProvider: clusterProvider,
                    identityLookup: new PartitionIdentityLookup()
                ).WithClusterKinds(actualClusterKinds(provider));

            // create the actor system

            return new ActorSystem(actorSystemConfig)
                .WithServiceProvider(provider)
                .WithRemote(remoteConfig)
                .WithCluster(clusterConfig);
        });
        SetupLogger();
    }

    public static void SetupLogger()
    {
        //Configure ProtoActor to use Console logger
        Proto.Log.SetLoggerFactory(
            LoggerFactory.Create(l => l.AddConsole().SetMinimumLevel(LogLevel.Warning)));
    }
}
