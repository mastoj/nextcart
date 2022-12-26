using FluentAssertions;

namespace NextCart.Api.Tests;

public class HelloTests : IClassFixture<PostgreSQLFixture>
{
    // private readonly TestApi _application;

    // public HelloTests(PostgreSQLFixture fixture)
    // {
    //     _application = new TestApi(fixture);
    // }
    // [Fact]
    // public async void GET_Root_Should_Return_Ok()
    // {
    //     var client = _application.CreateClient();
    //     var response = await client.GetStringAsync("/");
    //     response.Should().Be("Hello World!");
    // }
}