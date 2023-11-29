namespace Tests;

[TestFixture]
public class ProductServiceTests
{
    [Test]
    public async Task GetWith_ReturnsNoAccessToOperation_IfNoValidReadClaim()
    {
        // Implement this test
    }

    [Test]
    public async Task GetWith_ReturnsNotFound_IfValidClaimButNotExisting()
    {
        // Implement this test
    }

    [Test]
    public async Task GetWith_ReturnsNoAccessToData_IfNotValidMarket()
    {
        // Implement this test
    }

    [Test]
    public async Task GetWith_ReturnsOk_IfValidClaims()
    {
        // Implement this test
    }
}