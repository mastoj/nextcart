using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using NextCart.Api.Cart;
using NextCart.Api.Infrastructure;
using dotenv.net;
using Microsoft.AspNetCore.Diagnostics;
using Marten.Exceptions;

DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { ".env", ".env.local" }));
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMarten();
builder.Services.Configure<JsonOptions>(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapCart();
app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        var exceptionHandlerPathFeature =
            context.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature?.Error is ExistingStreamIdCollisionException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    });
});

app.Run();

public partial class Program
{
}
