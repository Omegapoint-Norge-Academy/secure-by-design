using SalesApi.Domain.DomainPrimitives;
using SalesApi.Domain.Model;
using SalesApi.Infrastructure;

namespace SalesApi.Domain.Services;

public class ProductService(IProductRepository productRepository, IPermissionService permissionService) : IProductService
{
    public async Task<(ReadDataResult, List<Product>?)> GetAllAvailableProducts()
    {
        if (!permissionService.CanReadProducts)
        {
            return (ReadDataResult.NoAccessToOperation, null);
        }

        var products = await productRepository.GetAllAvailable();

        var allowedProducts = products.Where(product => permissionService.HasPermissionToMarket(product.MarketId)).ToList();

        return (ReadDataResult.Success, allowedProducts);
    }

    public async Task<(ReadDataResult, Product?)> GetWith(ProductId id)
    {
        if (!permissionService.CanReadProducts)
        {
            return (ReadDataResult.NoAccessToOperation, null);
        }

        var product = await productRepository.GetBy(id);

        if (product is null) return (ReadDataResult.NotFound, null);

        if (!permissionService.HasPermissionToMarket(product.MarketId))
        {
            return (ReadDataResult.NoAccessToData, null);
        }

        return (ReadDataResult.Success, product);
    }
}