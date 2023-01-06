using System.Text;
using Marten.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NextCart.Api.Cart.Proto;
using Proto;
using Proto.Cluster;

namespace NextCart.Api.Cart;

#region requests
public record CreateCartRequest(string cartId);
public record AddItemRequest(string productId, int quantity);
#endregion

public static class CartApi
{
    public static RouteGroupBuilder MapCart(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/cart");
        group.AddEndpointFilter(async (invocationContext, next) =>
        {
            var body = await invocationContext.HttpContext.Request.BodyReader.ReadAsync();
            var bodyString = Encoding.UTF8.GetString(body.Buffer);
            Console.WriteLine($"====> Request: {invocationContext.HttpContext.Request.Path} {invocationContext.HttpContext.Request.Method} {bodyString}");
            return await next(invocationContext);
        });

        group.MapPost("/", CreateCart);
        group.MapGet("/{id}", GetCart);
        group.MapPost("/{id}/items", AddItem);
        group.MapPost("/{id}/clear", ClearCart);
        group.MapPost("/{id}/items/{productId}/increasequantity", IncreaseQuantity);
        group.MapPost("/{id}/items/{productId}/decreasequantity", DecreaseQuantity);
        group.MapDelete("/{id}/items/{productId}", RemoveItem);

        return group;
    }

    private static async Task<Created<CartDto>> CreateCart(ActorSystem actorSystem, [FromBody] CreateCartRequest request, CancellationToken ct)
    {
        try
        {
            Console.WriteLine("====> CreateCart: " + request.cartId);
            var grain = actorSystem.Cluster().GetCartGrain(request.cartId);
            var result = await grain.
                    Create(new Proto.CreateCart(), ct);
            return TypedResults.Created($"/cart/{result.Cart.Id}", result.Cart);
        }
        catch (ExistingStreamIdCollisionException)
        {
            throw new DuplicateCartException(request.cartId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"====> Exception: {ex}");
            throw;
        }
    }

    private static async Task<Ok<CartDto>> GetCart(ActorSystem actorSystem, [FromRoute] string id, CancellationToken ct)
    {
        try
        {
            var grain = actorSystem.Cluster().GetCartGrain(id);
            var result = await grain.Get(new Proto.GetCart(), ct);
            return TypedResults.Ok(result.Cart);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"====> Exception: {ex}");
            throw;
        }
    }

    private static async Task<Ok<CartDto>> AddItem(ActorSystem actorSystem, [FromRoute] string id, [FromBody] AddItemRequest request, CancellationToken ct)
    {
        try
        {
            var grain = actorSystem.Cluster().GetCartGrain(id);
            var result = await grain.AddItem(new Proto.AddItem { ProductId = request.productId }, ct);
            return TypedResults.Ok(result.Cart);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"====> Exception: {ex}");
            throw;
        }
    }

    private static async Task<Ok<CartDto>> IncreaseQuantity(ActorSystem actorSystem, [FromRoute] string id, [FromRoute] string productId, CancellationToken ct)
    {
        try
        {
            var grain = actorSystem.Cluster().GetCartGrain(id);
            var result = await grain.IncreaseQuantity(new Proto.IncreaseQuantity { ProductId = productId }, ct);
            return TypedResults.Ok(result.Cart);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"====> Exception: {ex}");
            throw;
        }
    }

    private static async Task<Ok<CartDto>> DecreaseQuantity(ActorSystem actorSystem, [FromRoute] string id, [FromRoute] string productId, CancellationToken ct)
    {
        try
        {
            var grain = actorSystem.Cluster().GetCartGrain(id);
            var result = await grain.DecreaseQuantity(new Proto.DecreaseQuantity { ProductId = productId }, ct);
            return TypedResults.Ok(result.Cart);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"====> Exception: {ex}");
            throw;
        }
    }

    private static async Task<Ok<CartDto>> RemoveItem(ActorSystem actorSystem, [FromRoute] string id, [FromRoute] string productId, CancellationToken ct)
    {
        try
        {
            var grain = actorSystem.Cluster().GetCartGrain(id);
            var result = await grain.RemoveItem(new Proto.RemoveItem { ProductId = productId }, ct);
            return TypedResults.Ok(result.Cart);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"====> Exception: {ex}");
            throw;
        }
    }

    private static async Task<Ok<CartDto>> ClearCart(ActorSystem actorSystem, [FromRoute] string id, CancellationToken ct)
    {
        try
        {
            var grain = actorSystem.Cluster().GetCartGrain(id);
            var result = await grain.Clear(new Proto.Clear(), ct);
            return TypedResults.Ok(result.Cart);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"====> Exception: {ex}");
            throw;
        }
    }
}