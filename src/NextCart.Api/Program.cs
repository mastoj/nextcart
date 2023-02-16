using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using NextCart.Api.Cart;
using NextCart.Api.Infrastructure;
using dotenv.net;
using Microsoft.AspNetCore.Diagnostics;
using NextCart.Domain.Infrastructure;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using System.Diagnostics.Metrics;
using Npgsql;
using Honeycomb.OpenTelemetry;
using Proto.OpenTelemetry;
;

DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { ".env", ".env.local" }));
var builder = WebApplication.CreateBuilder(args);
var nextCartMode = Environment.GetEnvironmentVariable("NEXTCART_MODE") ?? "local";
if (nextCartMode == "local")
{
    builder.Services.AddMarten();
    builder.Services.AddTestActorSystem();
}
else if (nextCartMode == "kubernetes")
{
    builder.Services.AddKubernetesActorSystem(Environment.GetEnvironmentVariable("ProtoActor__AdvertisedHost")!);
}
else if (nextCartMode == "docker")
{
    builder.Services.AddDockerActorSystem(Environment.GetEnvironmentVariable("PROTO_SEED_HOST")!);
}
else
{
    throw new System.Exception("NEXTCART_MODE is not set to local, kubernetes or docker");
}
builder.Services.AddHostedService<ActorSystemClusterClientHostedService>();
builder.Services.Configure<JsonOptions>(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Add open telemetry
var serviceName = "NextCart.Api";
var serviceVersion = "1.0.0";

var meter = new Meter(serviceName);
var counter = meter.CreateCounter<long>("app.request-counter");

var appResourceBuilder = ResourceBuilder.CreateDefault()
    .AddService(serviceName: serviceName, serviceVersion: serviceVersion);



var honeycombOptions = new HoneycombOptions
{
    ApiKey = Environment.GetEnvironmentVariable("HONEYCOMB_API_KEY")!,
    Dataset = Environment.GetEnvironmentVariable("HONEYCOMB_DATASET")!,
    ServiceName = serviceName,
    ServiceVersion = serviceVersion,
    ResourceBuilder = appResourceBuilder,
};
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .AddConsoleExporter()
            .AddSource(serviceName)
            .AddProtoActorInstrumentation()
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
            .AddNpgsql()
            .AddGrpcClientInstrumentation()
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddSqlClientInstrumentation()
            .AddHoneycomb(honeycombOptions);
        //            .AddAutoInstrumentations();
    })
    .WithMetrics(metricProviderBuilder =>
    {
        metricProviderBuilder
            .AddConsoleExporter()
            .AddPrometheusExporter()
            .AddMeter(meter.Name)
            .SetResourceBuilder(appResourceBuilder)
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();
    });

var app = builder.Build();
app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.MapGet("/", () => "Hello World!");

app.MapCart();
app.UseExceptionHandler(
    new ExceptionHandlerOptions
    {
        AllowStatusCode404Response = true,
        ExceptionHandler = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                var exceptionHandlerPathFeature =
                    context.Features.Get<IExceptionHandlerPathFeature>();
                if (exceptionHandlerPathFeature?.Error is ApiException)
                {
                    var exception = exceptionHandlerPathFeature.Error as ApiException;
                    context.Response.StatusCode = exception!.errorDto.HttpErrorCode;
                    await context.Response.WriteAsJsonAsync(exception.errorDto);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                }
            }
    });

app.Run();

public partial class Program
{
}
