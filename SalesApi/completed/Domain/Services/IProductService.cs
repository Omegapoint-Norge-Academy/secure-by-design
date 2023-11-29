using SalesApi.Domain.DomainPrimitives;
using SalesApi.Domain.Model;

namespace SalesApi.Domain.Services;

public interface IProductService
{
    Task<(ReadDataResult, List<Product>?)> GetAllAvailableProducts();
    Task<(ReadDataResult, Product?)> GetWith(ProductId id);
    Task<(WriteDataResult, Product?)> UpdatePrice(ProductId id, Money newPrice);
    Task<(WriteDataResult, Product?)> CreateNewProduct(Product product);
}
