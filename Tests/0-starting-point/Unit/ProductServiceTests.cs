using NSubstitute;
using SalesApi.Domain.DomainPrimitives;
using SalesApi.Domain.Model;
using SalesApi.Domain.Services;
using SalesApi.Infrastructure;

namespace Tests.Unit;

public class ProductServiceTests
{
    [Fact]
    public async Task GetWith_ReturnsNoAccessToOperation_IfNoValidReadClaim()
    {
        // Implement this test
        Assert.Fail();
    }

    [Fact]
    public async Task GetWith_ReturnsNotFound_IfValidClaimButNotExisting()
    {
        // Implement this test
        Assert.Fail();
    }

    [Fact]
    public async Task GetWith_ReturnsNoAccessToData_IfNotValidMarket()
    {
        // Implement this test
        Assert.Fail();
    }

    [Fact]
    public async Task GetWith_ReturnsOk_IfValidClaims()
    {
        // Implement this test
        Assert.Fail();
    }
}