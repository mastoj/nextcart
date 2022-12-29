using System.Net;
using FluentAssertions;
using Newtonsoft.Json;
using NextCart.Api.Cart;

namespace NextCart.Api.Tests;

public class CreateCartTests : TestApi
{
    private HttpClient _client;

    public CreateCartTests(PostgreSQLFixture fixture) : base(fixture)
    {
        _client = CreateClient();
    }

    private static CreateCartRequest ValidRequest() => new(Guid.NewGuid());

    [Fact]
    public async void Valid_Cart_Returns_Created_Cart()
    {
        var request = ValidRequest();
        var response = await _client.PostAsJsonAsync("/cart", request);
        response!.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async void Valid_Cart_Returns_Locatoin_Of_Cart()
    {
        var request = ValidRequest();
        var response = await _client.PostAsJsonAsync("/cart", request);
        response!.Headers.First(y => y.Key == "Location").Value.First().Should().Be($"/cart/{request.cartId}");
    }

    [Fact]
    public async void Duplicate_Cart_Id_Returns_Bad_Request()
    {
        var request = ValidRequest();
        _ = await _client.PostAsJsonAsync("/cart", request);
        var response = await _client.PostAsJsonAsync("/cart", request);
        response!.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}