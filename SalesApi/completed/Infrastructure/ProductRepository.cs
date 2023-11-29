using SalesApi.Domain.DomainPrimitives;
using SalesApi.Domain.Model;

namespace SalesApi.Infrastructure;

public class ProductRepository : IProductRepository
{
    private readonly List<Product> _products = [
        new(
            new ProductId("123GQWE"),
            new ProductName("Product 2"),
            new Money(12.5m, "NOK"),
            new MarketId("no")),

        new(
            new ProductId("234QWE"),
            new ProductName("Product 1"),
            new Money(9m, "USD"),
            new MarketId("no")),
        new(
            new ProductId("345XCV"),
            new ProductName("Product 3"),
            new Money(112m, "NOK"),
            new MarketId("no")),
        new(
            new ProductId("459KLP"),
            new ProductName("Product 4"),
            new Money(58m, "NOK"),
            new MarketId("no")),
    ];

    public Task<List<Product>> GetAllAvailable()
    {
        return Task.FromResult(_products);
    }

    public Task<Product?> GetBy(ProductId productId)
    {
        return Task.FromResult(_products.FirstOrDefault(p => p.Id == productId));
    }

    public Task SaveProduct(Product product)
    {
        if (_products.Any(x => x.Id == product.Id))
        {
            _products.Remove(product);
            _products.Add(product);
            return Task.CompletedTask;
        }
        _products.Add(product);
        return Task.CompletedTask;
    }
}