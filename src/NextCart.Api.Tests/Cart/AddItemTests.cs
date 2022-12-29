using System.Net;
using FluentAssertions;
using NextCart.Api.Cart;

namespace NextCart.Api.Tests;

public class AddItemTests : TestApi
{
    public AddItemTests(PostgreSQLFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async void Fails_For_Missing_Cart()
    {
        var productId = Guid.NewGuid().ToString();
        var request = new AddItemRequest(productId, 1);
        var response = await Client.PostAsJsonAsync($"/cart/{Guid.NewGuid()}/items", request);
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async void Succeed_For_Valid_Cart_And_Valid_Product_Id()
    {
        var cartRequest = CreateCartTests.ValidRequest();
        var _ = await Client.PostAsJsonAsync("/cart", cartRequest);

        var productId = Guid.NewGuid().ToString();
        var request = new AddItemRequest(productId, 1);
        var response = await Client.PostAsJsonAsync($"/cart/{cartRequest.cartId}/items", request);
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}