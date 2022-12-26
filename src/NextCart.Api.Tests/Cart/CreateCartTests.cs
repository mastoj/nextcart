using System.Net;
using FluentAssertions;
using NextCart.Api.Cart;

namespace NextCart.Api.Tests;

public class CreateCartTests : TestApi, IClassFixture<PostgreSQLFixture>
{
    private HttpClient _client;

    public CreateCartTests(PostgreSQLFixture fixture) : base(fixture)
    {
        _client = CreateClient();
    }

    private static CreateCartRequest ValidRequest() => new(Guid.NewGuid());

    [Fact]
    public async void Create_For_Valid_Cart_Returns_Created_Cart()
    {
        var request = ValidRequest();
        var response = await _client.PostAsJsonAsync("/cart", request);
        response!.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    // [Fact]
    // public async void Create_For_Valid_Cart_Returns_Locatin_Of_Cart()
    // {
    //     var request = ValidRequest();
    //     var response = await _client.PostAsJsonAsync("/cart", request);
    //     response!.StatusCode.Should().Be(HttpStatusCode.Created);
    //     response!.Headers.First(y => y.Key == "Location").Value.First().Should().Be($"/cart/{request.cartId}");
    // }
}