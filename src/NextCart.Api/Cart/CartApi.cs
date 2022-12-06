using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Cart;

public static class CartApi
{
    public static RouteGroupBuilder MapCart(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/cart");

        group.MapGet("/{id}", GetCart);

        return group;
    }

    public static Results<Ok<string>, NotFound> GetCart(string id)
    {
        if (id == "1")
        {
            return TypedResults.Ok("Cart 1");
        }
        else return TypedResults.NotFound();
    }
}