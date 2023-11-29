using System.Net;

namespace Tests.System;

[TestFixture]
public class ProductTests : BaseTests
{
    [Test]
    public async Task GetById_ShouldReturn401_WhenAnonymous()
    {
        var response = await _client.GetAsync("api/product/123GQWE");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GetProductById_ShouldReturn403_WhenAuthorizedButHasWrongScope()
    {
        await AuthorizeHttpClient(ProductScope.Write);
        // Use a token with wrong scope, GetProductById requires products.read
        var response = await _client.GetAsync("/api/product/123GQWE");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    public async Task GetProductById_ShouldReturn200_WhenAuthorizedButHasCorrectScope()
    {
        await AuthorizeHttpClient(ProductScope.Read);
        var response = await _client.GetAsync("/api/product/123GQWE");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}
