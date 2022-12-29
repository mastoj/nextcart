using Marten;
using Marten.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NextCart.Api.Infrastructure;

namespace NextCart.Api.Cart;

public record CartDto(Guid cartId);

#region requests
public record CreateCartRequest(Guid cartId);
#endregion

public static class CartApi
{
    public static RouteGroupBuilder MapCart(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/cart");

        group.MapPost("/", CreateCart);
        // group.MapGet("/{id}", GetCart);
        // group.MapPost("/{id}/clear", ClearCart);

        // group.MapPost("/{id}/items", AddItem);
        // group.MapPut("/{id}/items/{itemId}", UpdateItem);
        // group.MapDelete("/{id}/items/{itemId}", RemoveItem);

        return group;
    }

    private static async Task<Results<Created<CartDto>, BadRequest>> CreateCart(IDocumentSession documentSession, [FromBody] CreateCartRequest request, CancellationToken ct)
    {
        try
        {
            var result = await documentSession.Add<Cart>(request.cartId, () => CartService.Handle(new CreateCart(request.cartId)), ct);
            var cart = new CartDto(result.Id);
            return TypedResults.Created($"/cart/{result.Id}", cart);
        }
        catch (ExistingStreamIdCollisionException)
        {
            return TypedResults.BadRequest();
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