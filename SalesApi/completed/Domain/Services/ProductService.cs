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

        var allowedProducts = products.Where(product => permissionService.HasPermissionToMarket(product.MarketId));

        return (ReadDataResult.Success, products);
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

    public async Task<(WriteDataResult, Product?)> UpdatePrice(ProductId id, Money newPrice)
    {
        if (!permissionService.CanWriteProducts)
        {
            return (WriteDataResult.NoAccessToOperation, null);
        }

        var product = await productRepository.GetBy(id);

        if (product is null) return (WriteDataResult.NotFound, null);

        if (!permissionService.HasPermissionToMarket(product.MarketId))
        {
            return (WriteDataResult.NoAccessToData, null);
        }

        var (result, updatedProduct) = product.UpdatePrice(newPrice);

        if (result != WriteDataResult.Success || updatedProduct is null) return (result, null);

        await productRepository.SaveProduct(updatedProduct);

        return (WriteDataResult.Success, updatedProduct);
    }

    public async Task<(WriteDataResult, Product?)> CreateNewProduct(Product product)
    {
        if (!permissionService.CanWriteProducts)
        {
            return (WriteDataResult.NoAccessToOperation, null);
        }

        await productRepository.SaveProduct(product);
        return (WriteDataResult.Success, product);
    }
}