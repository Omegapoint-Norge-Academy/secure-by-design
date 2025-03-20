using System.Net;
using Xunit.Abstractions;

namespace Tests.System;

public class ProductTests(ITestOutputHelper testOutput) : BaseTests(testOutput)
{
    [Fact]
    public async Task GetById_ShouldReturn401_WhenAnonymous()
    {
        var response = await _client.GetAsync("api/product/123GQWE");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProductById_ShouldReturn403_WhenAuthorizedButHasWrongScope()
    {
        await AuthorizeHttpClient(ProductScope.Write);
        // Use a token with wrong scope, GetProductById requires products.read
        var response = await _client.GetAsync("/api/product/123GQWE");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetProductById_ShouldReturn200_WhenAuthorizedAndHasCorrectScope()
    {
        await AuthorizeHttpClient(ProductScope.Read);
        var response = await _client.GetAsync("/api/product/123GQWE");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}