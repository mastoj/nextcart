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
        _ = serviceCollection.AddSingleton(provider =>
        {
            // actor system configuration
            var actorSystemConfig = ActorSystemConfig
                .Setup();

            var remoteConfig = GrpcNetRemoteConfig
                .BindToLocalhost();

            IClusterProvider clusterProvider = new TestProvider(new TestProviderOptions(), new InMemAgent());

            var clusterConfig = ClusterConfig
                .Setup(
                    clusterName: "NextCart",
                    clusterProvider: clusterProvider,
                    identityLookup: new PartitionIdentityLookup()
                )
                .WithClusterKind(
                    kind: CartGrainActor.Kind,
                    prop: Props.FromProducer(() =>
                    new CartGrainActor((context, clusterIdentity) =>
                        ActivatorUtilities.CreateInstance<CartGrain>(provider, context)))
                );

            return new ActorSystem(actorSystemConfig)
                .WithServiceProvider(provider)
                .WithRemote(remoteConfig)
                .WithCluster(clusterConfig);
        });
        SetupLogger();
    }

    public static void AddActorSystem(this IServiceCollection serviceCollection, bool useKubernetes, string? advertisedHost = null)
    {
        serviceCollection.AddSingleton(provider =>
        {
            // actor system configuration

            var actorSystemConfig = ActorSystemConfig
                .Setup();

            // remote configuration

            // var remoteConfig = GrpcNetRemoteConfig
            //     .BindToLocalhost();
            var seedHost = Environment.GetEnvironmentVariable("PROTO_SEED_HOST") ?? "127.0.0.1";
            var remoteConfig =
                useKubernetes ?
                    GrpcNetRemoteConfig
                        .BindToAllInterfaces(advertisedHost: advertisedHost)
                        .WithProtoMessages(Contracts.Cart.Proto.CartMessagesReflection.Descriptor) :
                    seedHost == "127.0.0.1" ?
                        GrpcNetRemoteConfig.BindToLocalhost().WithProtoMessages(Contracts.Cart.Proto.CartMessagesReflection.Descriptor) :
                        GrpcNetRemoteConfig
                            .BindToAllInterfaces(advertisedHost: seedHost, port: 8090)
                            .WithProtoMessages(Contracts.Cart.Proto.CartMessagesReflection.Descriptor);

            IClusterProvider clusterProvider =
                useKubernetes ? new KubernetesProvider() : new SeedNodeClusterProvider(new SeedNodeClusterProviderOptions((seedHost, 8090)));

            // cluster configuration
            var clusterConfig = ClusterConfig
                .Setup(
                    clusterName: "NextCart",
                    clusterProvider: clusterProvider,
                    identityLookup: new PartitionIdentityLookup()
                );
            // .WithClusterKind(
            //     kind: CartGrainActor.Kind,
            //     prop: Props.FromProducer(() =>
            //     new CartGrainActor((context, clusterIdentity) =>
            //         ActivatorUtilities.CreateInstance<CartGrain>(provider, context)))
            // );

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
