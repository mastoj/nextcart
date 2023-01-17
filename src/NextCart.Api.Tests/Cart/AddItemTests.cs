using System.Net;
using FluentAssertions;
using NextCart.Api.Cart;
using NextCart.Contracts.Cart.Proto;

namespace NextCart.Api.Tests;

public record ProductDto
{
    public string ProductId { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Url { get; init; } = null!;
    public string MainImage { get; init; } = null!;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = null!;
}

public record ItemDto
{
    public ProductDto Product { get; init; } = null!;
    public int Quantity { get; init; }
    public decimal ItemTotal { get; init; }
}

public record CartDto
{
    public string Id { get; init; } = null!;
    public int Version { get; init; }
    public decimal Total { get; init; }
    public List<ItemDto> Items { get; init; } = new();
}

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

        var actualCart = await response.Content.ReadFromJsonAsync<CartDto>();
        // TODO: Need proper data for products and inject the product repository
        var expected = new CartDto
        {
            Id = cartRequest.cartId,
            Version = 2,
            Total = 100,
            Items = new List<ItemDto>
            {
                new ItemDto
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
                }
            }
        };
        actualCart.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async void Returns_The_Cart_With_The_Article_And_Updated_Quantity_And_Total_If_Added_Twice()
    {
        var cartRequest = CreateCartTests.ValidCreateCartRequest();
        var _ = await Client.PostAsJsonAsync("/cart", cartRequest);

        var productId = Guid.NewGuid().ToString();
        var request = new AddItemRequest(productId);
        _ = await Client.PostAsJsonAsync($"/cart/{cartRequest.cartId}/items", request);
        var response = await Client.PostAsJsonAsync($"/cart/{cartRequest.cartId}/items", request);

        var actualCart = await response.Content.ReadFromJsonAsync<CartDto>();
        // TODO: Need proper data for products and inject the product repository
        var expected = new CartDto
        {
            Id = cartRequest.cartId,
            Version = 3,
            Total = 200,
            Items = new List<ItemDto>
            {
                new ItemDto
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
                    ItemTotal = 200,
                    Quantity = 2
                }
            }
        };
        actualCart.Should().BeEquivalentTo(expected);
    }
}