using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Cart;

public record CartDto(Guid id);

#region requests
public record CreateCartRequest(Guid id);
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

    private static Results<Created<CartDto>, NotFound> CreateCart([FromBody] CreateCartRequest request)
    {
        var cart = new CartDto(request.id);
        return TypedResults.Created($"/cart/{request.id}", cart);
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