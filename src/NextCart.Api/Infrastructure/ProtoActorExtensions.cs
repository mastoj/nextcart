using Proto;
using Proto.Cluster;
using Proto.Cluster.Partition;
using Proto.Cluster.Seed;
using Proto.Cluster.Testing;
using Proto.DependencyInjection;
using Proto.Remote;
using Proto.Remote.GrpcNet;

namespace NextCart.Api.Infrastructure;

public static class ProtoActorExtensions
{
    public static void AddActorSystem(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(provider =>
        {
            // actor system configuration

            var actorSystemConfig = ActorSystemConfig
                .Setup();

            // remote configuration

            // var remoteConfig = GrpcNetRemoteConfig
            //     .BindToLocalhost();
            var remoteConfig = GrpcNetRemoteConfig.BindToLocalhost(8090).WithProtoMessages(NextCart.Contracts.CartMessagesReflection.Descriptor);

            // cluster configuration
            var clusterConfig = ClusterConfig
                .Setup(
                    clusterName: "NextCart",
                    clusterProvider: new SeedNodeClusterProvider(),
                    // clusterProvider: new TestProvider(new TestProviderOptions(), new InMemAgent()),
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
