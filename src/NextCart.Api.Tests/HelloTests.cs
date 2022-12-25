using FluentAssertions;

namespace NextCart.Api.Tests;

public class HelloTests
{
    [Fact]
    public async void GET_Root_Should_Return_Ok()
    {
        var application = new TestApi();
        var client = application.CreateClient();
        var response = await client.GetStringAsync("/");
        response.Should().Be("Hello World!");
    }
}