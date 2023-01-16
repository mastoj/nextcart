using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using NextCart.Api.Cart;
using NextCart.Api.Infrastructure;
using dotenv.net;
using Microsoft.AspNetCore.Diagnostics;
using NextCart.Domain.Infrastructure;

DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { ".env", ".env.local" }));
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMarten();
// builder.Services.AddActorSystem(bool.Parse(Environment.GetEnvironmentVariable("NEXTCART_USE_KUBERNETES") ?? "false"), Environment.GetEnvironmentVariable("ProtoActor__AdvertisedHost") ?? null);
builder.Services.AddTestActorSystem();
builder.Services.AddHostedService<ActorSystemClusterClientHostedService>();
builder.Services.Configure<JsonOptions>(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

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
