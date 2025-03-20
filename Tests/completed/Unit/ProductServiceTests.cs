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
        var productRepository = Substitute.For<IProductRepository>();
        var permissionService = Substitute.For<IPermissionService>();
        var productService = new ProductService(productRepository, permissionService);
        var productId = new ProductId("1jf3jlk2");

        var (result, product) = await productService.GetWith(productId);

        Assert.Equal(ReadDataResult.NoAccessToOperation, result);
        Assert.Null(product);
    }

    [Fact]
    public async Task GetWith_ReturnsNotFound_IfValidClaimButNotExisting()
    {
        var productRepository = Substitute.For<IProductRepository>();
        var permissionService = Substitute.For<IPermissionService>();
        permissionService
            .CanReadProducts
            .Returns(true);
        var productService = new ProductService(productRepository, permissionService);
        var productId = new ProductId("notfound");

        var (result, product) = await productService.GetWith(productId);

        Assert.Equal(ReadDataResult.NotFound, result);
        Assert.Null(product);
    }

    [Fact]
    public async Task GetWith_ReturnsNoAccessToData_IfNotValidMarket()
    {
        var productRepository = Substitute.For<IProductRepository>();
        var permissionService = Substitute.For<IPermissionService>();
        var productId = new ProductId("1jf3jlk2");
        var marketId = new MarketId("se");
        productRepository
            .GetBy(Arg.Is(productId))
            .Returns(
                new Product(
                    productId,
                    new ProductName("Product 1"),
                    new Money(9m, "USD"),
                    new MarketId("no")
                )
            );
        permissionService.CanReadProducts.Returns(true);
        permissionService.HasPermissionToMarket(Arg.Is(marketId)).Returns(false);
        var productService = new ProductService(productRepository, permissionService);

        var (result, product) = await productService.GetWith(productId);

        Assert.Equal(ReadDataResult.NoAccessToData, result);
        Assert.Null(product);
    }

    [Fact]
    public async Task GetWith_ReturnsOk_IfValidClaims()
    {
        var productRepository = Substitute.For<IProductRepository>();
        var permissionService = Substitute.For<IPermissionService>();
        var productId = new ProductId("1jf3jlk2");
        productRepository
            .GetBy(Arg.Is(productId))
            .Returns(
                new Product(
                    productId,
                    new ProductName("Product 1"),
                    new Money(9m, "USD"),
                    new MarketId("no")
                )
            );
        permissionService.CanReadProducts.Returns(true);
        permissionService.HasPermissionToMarket(Arg.Is(new MarketId("no"))).Returns(true);
        var productService = new ProductService(productRepository, permissionService);

        var (result, product) = await productService.GetWith(productId);

        Assert.Equal(ReadDataResult.Success, result);
        Assert.NotNull(product);
    }
}