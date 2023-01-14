// See https://aka.ms/new-console-template for more information
// Created hosted service


using System.Text.Json.Serialization;
using dotenv.net;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            services.AddActorSystem(bool.Parse(Environment.GetEnvironmentVariable("NEXTCART_USE_KUBERNETES") ?? "false"), Environment.GetEnvironmentVariable("ProtoActor__AdvertisedHost") ?? null);
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
