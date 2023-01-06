using System.Net;
using FluentAssertions;
using NextCart.Api.Cart;
using NextCart.Api.Cart.Proto;

namespace NextCart.Api.Tests;

public class CreateCartTests : TestApi
{
    public CreateCartTests(PostgreSQLFixture fixture) : base(fixture)
    {
    }

    public static CreateCartRequest ValidRequest() => new(Guid.NewGuid().ToString());

    [Fact]
    public async void Valid_Cart_Returns_Created_Cart()
    {
        var request = ValidRequest();
        var response = await Client.PostAsJsonAsync("/cart", request);
        response!.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async void Valid_Cart_Returns_Location_Of_Cart()
    {
        var request = ValidRequest();
        var response = await Client.PostAsJsonAsync("/cart", request);
        response!.Headers.First(y => y.Key == "Location").Value.First().Should().Be($"/cart/{request.cartId}");
    }

    [Fact]
    public async void Valid_Cart_Returns_Empty_Cart()
    {
        var request = ValidRequest();
        var response = await Client.PostAsJsonAsync("/cart", request);
        var actualCart = await response.Content.ReadFromJsonAsync<CartDto>();
        actualCart.Should().Be(new CartDto { Id = request.cartId });
    }

    [Fact]
    public async void Duplicate_Cart_Id_Returns_Bad_Request()
    {
        var request = ValidRequest();
        _ = await Client.PostAsJsonAsync("/cart", request);
        var response = await Client.PostAsJsonAsync("/cart", request);
        response!.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}