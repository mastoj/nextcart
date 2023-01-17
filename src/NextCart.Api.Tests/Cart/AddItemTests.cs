using System.Net;
using FluentAssertions;
using NextCart.Api.Cart;
using NextCart.Contracts.Cart.Proto;

namespace NextCart.Api.Tests;

public class AddItemTests : TestApi
{
    public AddItemTests(PostgreSQLFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async void Fails_For_Missing_Cart()
    {
        // var createCartRequest = CreateCartTests.ValidCreateCartRequest();
        // var _ = await Client.PostAsJsonAsync("/cart", createCartRequest);

        var productId = Guid.NewGuid().ToString();
        var request = new AddItemRequest(productId);
        var response = await Client.PostAsJsonAsync($"/cart/{Guid.NewGuid()}/items", request);
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async void Succeed_For_Valid_Cart_And_Valid_Product_Id()
    {
        var cartRequest = CreateCartTests.ValidCreateCartRequest();
        var _ = await Client.PostAsJsonAsync("/cart", cartRequest);

        var productId = Guid.NewGuid().ToString();
        var request = new AddItemRequest(productId);
        var response = await Client.PostAsJsonAsync($"/cart/{cartRequest.cartId}/items", request);
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async void Returns_The_Cart_With_The_Article_For_Valid_Cart_And_Valid_Product_Id()
    {
        var cartRequest = CreateCartTests.ValidCreateCartRequest();
        var _ = await Client.PostAsJsonAsync("/cart", cartRequest);

        var productId = Guid.NewGuid().ToString();
        var request = new AddItemRequest(productId);
        var response = await Client.PostAsJsonAsync($"/cart/{cartRequest.cartId}/items", request);
        var textResponse = await response.Content.ReadAsStringAsync();

        var actualCart = await response.Content.ReadFromJsonAsync<CartDto>();
        // TODO: Need proper data for products and inject the product repository
        actualCart.Should().Be(new CartDto { Id = cartRequest.cartId, Version = 2, Total = 100 });
        actualCart.Items.Should().HaveCount(1);
        actualCart.Items[0].Should().Be(new ItemDto
        {
            Product = new ProductDto
            {
                ProductId = productId,
                Name = "Product Name",
                Url = "https://www.google.com",
                MainImage = "https://www.google.com",
                Amount = 100,
                Currency = "USD",
            },
            ItemTotal = 100,
            Quantity = 1
        });
    }
}