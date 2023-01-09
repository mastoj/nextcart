using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NextCart.Contracts;
using NextCart.Service.Cart;
using Proto;
using Proto.Cluster;
using Proto.Cluster.Kubernetes;
using Proto.Cluster.Partition;
using Proto.Cluster.Seed;
using Proto.DependencyInjection;
using Proto.Remote;
using Proto.Remote.GrpcNet;
// using ProtosReflection = NextCart.Service.Cart.Proto.Prot;

namespace NextCart.Service.Infrastructure;

public static class ProtoActorExtensions
{
    public static void AddActorSystem(this IServiceCollection serviceCollection, bool useKubernetes, string? advertisedHost = null)
    {
        serviceCollection.AddSingleton(provider =>
        {
            // actor system configuration
            var actorSystemConfig = ActorSystemConfig
                .Setup();
            var remoteConfig =
                useKubernetes ?
                    GrpcNetRemoteConfig
                        .BindToAllInterfaces(advertisedHost: advertisedHost)
                        .WithProtoMessages(CartMessagesReflection.Descriptor) :
                    GrpcNetRemoteConfig.BindToLocalhost().WithProtoMessages(CartMessagesReflection.Descriptor);

            IClusterProvider clusterProvider =
                useKubernetes ? new KubernetesProvider() : new SeedNodeClusterProvider(new SeedNodeClusterProviderOptions(("127.0.0.1", 8090)));
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

    public static void SetupLogger()
    {
        //Configure ProtoActor to use Console logger
        Proto.Log.SetLoggerFactory(
            LoggerFactory.Create(l => l.AddConsole().SetMinimumLevel(LogLevel.Warning)));
    }
}
