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
}