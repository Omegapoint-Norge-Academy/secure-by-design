using Moq;
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
        var productRepository = Mock.Of<IProductRepository>(MockBehavior.Strict);
        var permissionService = Mock.Of<IPermissionService>();
        var productService = new ProductService(productRepository, permissionService);
        var productId = new ProductId("1jf3jlk2");

        var (result, product) = await productService.GetWith(productId);

        Assert.Equal(ReadDataResult.NoAccessToOperation, result);
        Assert.Null(product);
    }

    [Fact]
    public async Task GetWith_ReturnsNotFound_IfValidClaimButNotExisting()
    {
        var productRepository = Mock.Of<IProductRepository>();
        var permissionService = Mock.Of<IPermissionService>();
        Mock.Get(permissionService).SetupGet(service => service.CanReadProducts).Returns(true);
        var productService = new ProductService(productRepository, permissionService);
        var productId = new ProductId("notfound");

        var (result, product) = await productService.GetWith(productId);

        Assert.Equal(ReadDataResult.NotFound, result);
        Assert.Null(product);
    }

    [Fact]
    public async Task GetWith_ReturnsNoAccessToData_IfNotValidMarket()
    {
        var productRepository = Mock.Of<IProductRepository>();
        var permissionService = Mock.Of<IPermissionService>();
        var productId = new ProductId("1jf3jlk2");
        Mock.Get(productRepository)
            .Setup(repo => repo.GetBy(productId))
            .ReturnsAsync(
                new Product(
                    productId,
                    new ProductName("Product 1"),
                    new Money(9m, "USD"),
                    new MarketId("no")
                )
            );
        Mock.Get(permissionService).SetupGet(service => service.CanReadProducts).Returns(true);
        Mock.Get(permissionService).Setup(service => service.HasPermissionToMarket(new MarketId("se"))).Returns(false);
        var productService = new ProductService(productRepository, permissionService);

        var (result, product) = await productService.GetWith(productId);

        Assert.Equal(ReadDataResult.NoAccessToData, result);
        Assert.Null(product);
    }

    [Fact]
    public async Task GetWith_ReturnsOk_IfValidClaims()
    {
        var productRepository = Mock.Of<IProductRepository>();
        var permissionService = Mock.Of<IPermissionService>();
        var productId = new ProductId("1jf3jlk2");
        Mock.Get(productRepository)
            .Setup(repo => repo.GetBy(productId))
            .ReturnsAsync(
                new Product(
                    productId,
                    new ProductName("Product 1"),
                    new Money(9m, "USD"),
                    new MarketId("no")
                )
            );
        Mock.Get(permissionService).SetupGet(service => service.CanReadProducts).Returns(true);
        Mock.Get(permissionService).Setup(service => service.HasPermissionToMarket(new MarketId("no"))).Returns(true);
        var productService = new ProductService(productRepository, permissionService);

        var (result, product) = await productService.GetWith(productId);

        Assert.Equal(ReadDataResult.Success, result);
        Assert.NotNull(product);
    }
}