using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Proto.Cluster;
using Proto;
using NextCart.Contracts.Cart.Proto;
using Polly;

namespace NextCart.Api.Cart;

#region requests
public record CreateCartRequest(string cartId);
public record AddItemRequest(string productId);
#endregion

public static class CartApi
{
    private static AsyncPolicy _policy =
        Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    public static RouteGroupBuilder MapCart(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/cart");
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
            var result = await
                _policy.ExecuteAsync(async () => await grain.Create(new CreateCart { Id = request.cartId }, ct));
            return TypedResults.Created($"/cart/{result.Cart.Id}", result.Cart);
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
            var result = await
                _policy.ExecuteAsync(async () => await grain.Get(new GetCart(), ct));
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
            var result =
                await _policy.ExecuteAsync(async () => await grain.AddItem(new AddItem { ProductId = request.productId }, ct));
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
            var result = await
                _policy.ExecuteAsync(async () => await grain.IncreaseQuantity(new IncreaseQuantity { ProductId = productId }, ct));
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
            var result = await
                _policy.ExecuteAsync(async () => await grain.DecreaseQuantity(new DecreaseQuantity { ProductId = productId }, ct));
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
            var result = await
                _policy.ExecuteAsync(async () => await grain.RemoveItem(new RemoveItem { ProductId = productId }, ct));
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
            var result = await grain.Clear(new Clear(), ct);
            return TypedResults.Ok(result.Cart);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"====> Exception: {ex}");
            throw;
        }
    }
}