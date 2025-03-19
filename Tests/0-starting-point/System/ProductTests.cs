using Xunit.Abstractions;

namespace Tests.System;

public class ProductTests(ITestOutputHelper testOutput) : BaseTests(testOutput)
{
    [Fact]
    public async Task GetById_ShouldReturn401_WhenAnonymous()
    {
        // Implement this test
        Assert.Fail();
    }

    [Fact]
    public async Task GetProductById_ShouldReturn403_WhenAuthorizedButHasWrongScope()
    {
        // Implement this test
        Assert.Fail();
    }

    [Fact]
    public async Task GetProductById_ShouldReturn200_WhenAuthorizedAndHasCorrectScope()
    {
        // Implement this test
        Assert.Fail();
    }
}