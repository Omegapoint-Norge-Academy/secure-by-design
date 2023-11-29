namespace Tests.System;

[TestFixture]
public class ProductTests : BaseTests
{
    [Test]
    public async Task GetById_ShouldReturn401_WhenAnonymous()
    {
        // Implement this test
    }

    [Test]
    public async Task GetProductById_ShouldReturn403_WhenAuthorizedButHasWrongScope()
    {
        // Implement this test
    }

    [Test]
    public async Task GetProductById_ShouldReturn200_WhenAuthorizedButHasCorrectScope()
    {
        // Implement this test
    }
}
