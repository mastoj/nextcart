using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using NextCart.Api.Cart;
using NextCart.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMarten();
builder.Services.Configure<JsonOptions>(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapCart();

app.Run();

public partial class Program
{
}
