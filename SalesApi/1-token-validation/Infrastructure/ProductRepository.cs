using SalesApi.Domain.Model;

namespace SalesApi.Infrastructure;

public class ProductRepository : IProductRepository
{
    private readonly List<Product> _products = [
        new(
            "123GQWE",
            "Product 2",
            12.5m,
            "no"),
        new(
            "234QWE",
            "Product 1",
            9m,
            "no"),
    ];

    public Task<List<Product>> GetAllAvailable()
    {
        return Task.FromResult(_products);
    }

    public Task<Product?> GetBy(string productId)
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