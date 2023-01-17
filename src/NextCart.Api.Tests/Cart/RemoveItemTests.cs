using System.Net;
using FluentAssertions;
using NextCart.Api.Cart;
using NextCart.Contracts.Cart.Proto;

namespace NextCart.Api.Tests;


public class RemoveItemTests : TestApi
{
    public RemoveItemTests(PostgreSQLFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async void Fails_For_Missing_Cart()
    {
        var productId = Guid.NewGuid().ToString();
        var response = await Client.DeleteAsync($"/cart/{Guid.NewGuid()}/items/{productId}");
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async void Succeed_For_Valid_Cart_And_Valid_Product_Id()
    {
        var productId = Guid.NewGuid().ToString();
        var cartRequest = CreateCartTests.ValidCreateCartRequest();
        var _ = await Client.PostAsJsonAsync("/cart", cartRequest);
        var addRequest = new AddItemRequest(productId);
        _ = await Client.PostAsJsonAsync($"/cart/{cartRequest.cartId}/items", addRequest);

        var response = await Client.DeleteAsync($"/cart/{cartRequest.cartId}/items/{productId}");
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}