using Marten;
using Marten.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NextCart.Api.Infrastructure;

namespace NextCart.Api.Cart;

public record ProductDto(
    string productId,
    string name,
    string url,
    string mainImage,
    float amount,
    string currency,
    int quantity);

public record CartDto(
    Guid cartId,
    IEnumerable<ProductDto>? items = null,
    float total = 0);

#region requests
public record CreateCartRequest(Guid cartId);
public record AddItemRequest(string productId, int quantity);
#endregion

public static class CartApi
{
    public static RouteGroupBuilder MapCart(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/cart");

        group.MapPost("/", CreateCart);
        group.MapPost("/{id}/items", AddItem);
        // group.MapGet("/{id}", GetCart);
        // group.MapPost("/{id}/clear", ClearCart);

        // group.MapPost("/{id}/items", AddItem);
        // group.MapPut("/{id}/items/{itemId}", UpdateItem);
        // group.MapDelete("/{id}/items/{itemId}", RemoveItem);

        return group;
    }

    private static async Task<Created<CartDto>> CreateCart(IDocumentSession documentSession, [FromBody] CreateCartRequest request, CancellationToken ct)
    {
        try
        {
            var result = await documentSession.Add<Cart>(request.cartId, () => CartService.Handle(new CreateCart(request.cartId)), ct);
            var cart = new CartDto(result.Id);
            return TypedResults.Created($"/cart/{result.Id}", cart);
        }
        catch (ExistingStreamIdCollisionException)
        {
            throw new DuplicateCartException(request.cartId);
        }
    }

    private static async Task<Ok<CartDto>> AddItem(IDocumentSession documentSession, [FromRoute] Guid id, [FromBody] AddItemRequest request, CancellationToken ct)
    {
        try
        {
            var product = new Product(request.productId, "name", "url", "mainImage", 1, "EUR", request.quantity);
            var result = await documentSession.GetAndUpdate<Cart>(id, 1, cart => CartService.Handle(cart, new AddItem(product)), ct);
            var cart = new CartDto(result.Id, result.Items?.Select(x => new ProductDto(x.ProductId, x.Name, x.Url, x.MainImage, x.Amount, x.Currency, x.Quantity)), result.Total);
            return TypedResults.Ok(cart);
        }
        catch (ConcurrencyException ex)
        {
            throw new CartNotFoundException(id);
        }
    }

    //     private static Results<Ok<CartDto>, NotFound> GetCart([FromBody] CreateCartRequest request)
    //     {
    //         var cart = new Cart();
    //         return TypedResults.Ok(cart);
    //     }

    //     private static async Task ClearCart(HttpContext context)
    //     {
    //         await context.Response.WriteAsJsonAsync(new Cart());
    //     }

    //     private static Results<Ok<string>, NotFound> AddItem()
    //     {
    //         return TypedResults.Ok("Hello");
    //     }

    //     private static Results<Ok<string>, NotFound> UpdateItem()
    //     {
    //         return TypedResults.Ok("Hello");
    //     }

    //     private static Results<Ok<string>, NotFound> RemoveItem()
    //     {
    //         return TypedResults.Ok("Hello");
    //     }
}