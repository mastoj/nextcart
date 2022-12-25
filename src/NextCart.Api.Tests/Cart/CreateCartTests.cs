using System.Net;
using Cart;
using FluentAssertions;

namespace NextCart.Api.Tests;

public class CreateCartTests
{
    private HttpClient _client;

    public CreateCartTests()
    {
        var application = new TestApi();
        _client = application.CreateClient();
    }

    private static CreateCartRequest ValidRequest() => new(Guid.NewGuid());

    [Fact]
    public async void Create_For_Valid_Cart_Returns_Created_Cart()
    {
        var request = ValidRequest();
        var response = await _client.PostAsJsonAsync("/cart", request);
        response!.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async void Create_For_Valid_Cart_Returns_Locatin_Of_Cart()
    {
        var request = ValidRequest();
        var response = await _client.PostAsJsonAsync("/cart", request);
        response!.StatusCode.Should().Be(HttpStatusCode.Created);
        response!.Headers.First(y => y.Key == "Location").Value.First().Should().Be($"/cart/{request.id}");
    }
}