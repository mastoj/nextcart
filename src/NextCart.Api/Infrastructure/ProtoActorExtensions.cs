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
            var remoteConfig =
                useKubernetes ?
                    GrpcNetRemoteConfig
                        .BindToAllInterfaces(advertisedHost: advertisedHost)
                        .WithProtoMessages(Contracts.CartMessagesReflection.Descriptor) :
                    GrpcNetRemoteConfig.BindToLocalhost(8090).WithProtoMessages(Contracts.CartMessagesReflection.Descriptor);

            IClusterProvider clusterProvider =
                useKubernetes ? new KubernetesProvider() : new SeedNodeClusterProvider(new SeedNodeClusterProviderOptions(("127.0.0.1", 8090)));

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
