using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Proto.Cluster;
using Proto;
using NextCart.Contracts.Cart.Proto;
using Polly;
using NextCart.Api.Infrastructure;

namespace NextCart.Api.Cart;

#region requests
public record CreateCartRequest(string cartId);
public record AddItemRequest(string productId);
public record SetShippingAddressRequest(string addressId, string firstName, string lastName, string addressLine1, string addressLine2, string city, string postalCode, string country, string email);
public record AddPaymentMethodRequest(string paymentMethod, Dictionary<string, string> paymentMethodData);

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
        group.MapPost("/{id}/shippingaddress", AddShippingAddress);
        group.MapDelete("/{id}/items/{productId}", RemoveItem);

        return group;
    }

    private static async Task<Ok<CartDto>> AddShippingAddress(ActorSystem actorSystem, IRootContext rootContext, [FromRoute] string id, [FromBody] SetShippingAddressRequest request, CancellationToken ct)
    {
        try
        {
            var grain = actorSystem.Cluster().GetCartGrain(id);
            var result = await
                _policy.ExecuteAsync(async () => await grain.SetShippingAddress(new SetShippingAddress
                {
                    AddressId = request.addressId,
                    FirstName = request.firstName,
                    LastName = request.lastName,
                    AddressLine1 = request.addressLine1,
                    AddressLine2 = request.addressLine2,
                    City = request.city,
                    PostalCode = request.postalCode,
                    Country = request.country,
                    Email = request.email
                }, rootContext, ct));
            if (result?.Cart is not null)
            {
                return TypedResults.Ok(result.Cart);
            }
            throw new ApiException(result!.Error!);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"====> Exception: {ex}");
            throw;
        }
    }

    private static async Task<Created<CartDto>> CreateCart(ActorSystem actorSystem, IRootContext rootContext, [FromBody] CreateCartRequest request, CancellationToken ct)
    {
        Console.WriteLine("====> CreateCart: " + request.cartId);
        var grain = actorSystem.Cluster().GetCartGrain(request.cartId);
        var result = await
            _policy.ExecuteAsync(async () => await grain.Create(new CreateCart { Id = request.cartId }, rootContext, ct));
        if (result?.Cart is not null)
        {
            return TypedResults.Created($"/cart/{result.Cart.Id}", result.Cart);
        }
        throw new ApiException(result!.Error!);
    }

    private static async Task<Ok<CartDto>> GetCart(ActorSystem actorSystem, IRootContext rootContext, [FromRoute] string id, CancellationToken ct)
    {
        try
        {
            var grain = actorSystem.Cluster().GetCartGrain(id);
            var result = await
                _policy.ExecuteAsync(async () => await grain.Get(new GetCart(), rootContext, ct));
            return TypedResults.Ok(result.Cart);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"====> Exception: {ex}");
            throw;
        }
    }

    private static async Task<Ok<CartDto>> AddItem(ActorSystem actorSystem, IRootContext rootContext, [FromRoute] string id, [FromBody] AddItemRequest request, CancellationToken ct)
    {
        try
        {
            var grain = actorSystem.Cluster().GetCartGrain(id);
            var result =
                await _policy.ExecuteAsync(async () => await grain.AddItem(new AddItem { ProductId = request.productId }, rootContext, ct));
            if (result?.Cart is not null)
            {
                return TypedResults.Ok(result.Cart);
            }
            throw new ApiException(result!.Error!);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"====> Exception: {ex}");
            throw;
        }
    }

    private static async Task<Ok<CartDto>> IncreaseQuantity(ActorSystem actorSystem, IRootContext rootContext, [FromRoute] string id, [FromRoute] string productId, CancellationToken ct)
    {
        try
        {
            var grain = actorSystem.Cluster().GetCartGrain(id);
            var result = await
                _policy.ExecuteAsync(async () => await grain.IncreaseQuantity(new IncreaseQuantity { ProductId = productId }, rootContext, ct));
            return TypedResults.Ok(result.Cart);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"====> Exception: {ex}");
            throw;
        }
    }

    private static async Task<Ok<CartDto>> DecreaseQuantity(ActorSystem actorSystem, IRootContext rootContext, [FromRoute] string id, [FromRoute] string productId, CancellationToken ct)
    {
        try
        {
            var grain = actorSystem.Cluster().GetCartGrain(id);
            var result = await
                _policy.ExecuteAsync(async () => await grain.DecreaseQuantity(new DecreaseQuantity { ProductId = productId }, rootContext, ct));
            if (result?.Cart is not null)
            {
                return TypedResults.Ok(result.Cart);
            }
            throw new ApiException(result!.Error!);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"====> Exception: {ex}");
            throw;
        }
    }

    private static async Task<Ok<CartDto>> RemoveItem(ActorSystem actorSystem, IRootContext rootContext, [FromRoute] string id, [FromRoute] string productId, CancellationToken ct)
    {
        try
        {
            var grain = actorSystem.Cluster().GetCartGrain(id);
            var result = await
                _policy.ExecuteAsync(async () => await grain.RemoveItem(new RemoveItem { ProductId = productId }, rootContext, ct));
            if (result?.Cart is not null)
            {
                return TypedResults.Ok(result.Cart);
            }
            throw new ApiException(result!.Error!);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"====> Exception: {ex}");
            throw;
        }
    }

    private static async Task<Ok<CartDto>> ClearCart(ActorSystem actorSystem, IRootContext rootContext, [FromRoute] string id, CancellationToken ct)
    {
        try
        {
            var grain = actorSystem.Cluster().GetCartGrain(id);
            var result = await grain.Clear(new Clear(), rootContext, ct);
            return TypedResults.Ok(result.Cart);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"====> Exception: {ex}");
            throw;
        }
    }
}
