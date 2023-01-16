// See https://aka.ms/new-console-template for more information
// Created hosted service


using System.Text.Json.Serialization;
using dotenv.net;
using Microsoft.AspNetCore.Http.Json;
using NextCart.Domain.Infrastructure;
using NextCart.Service;
using NextCart.Service.Infrastructure;

DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { ".env", ".env.local" }));

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseConsoleLifetime()
        // .ConfigureLogging(builder => builder.SetMinimumLevel(LogLevel.Warning))
        .ConfigureServices((hostContext, services) =>
        {
            services.AddMarten();
            var nextCartMode = Environment.GetEnvironmentVariable("NEXTCART_MODE") ?? "local";
            Console.WriteLine("==> NEXTCART_MODE: " + nextCartMode);
            if (nextCartMode == "kubernetes")
            {
                services.AddActorSystem(true, Environment.GetEnvironmentVariable("ProtoActor__AdvertisedHost") ?? null);
            }
            else if (nextCartMode == "docker")
            {
                services.AddActorSystem(false, Environment.GetEnvironmentVariable("ProtoActor__AdvertisedHost") ?? null);
            }
            else
            {
                throw new System.Exception("NEXTCART_MODE is not set to local, kubernetes or docker");
            }
            services.AddHostedService<ActorSystemClusterHostedService>();
            services.Configure<JsonOptions>(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            // services.Configure<MyServiceOptions>(hostContext.Configuration);
            // services.AddHostedService<MyService>();
            // services.AddSingleton(Console.Out);
        });

using (var host = CreateHostBuilder(args).Build())
{
    await host.StartAsync();
    // var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

    // do work here / get your work service ...

    // lifetime.StopApplication();
    await host.WaitForShutdownAsync();
}

Console.WriteLine("Hello, World!");
