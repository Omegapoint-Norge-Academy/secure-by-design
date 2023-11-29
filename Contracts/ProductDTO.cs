
namespace Contracts;
public class ProductDTO
{
    public ProductDTO()
    {
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; }
    public string MarketId { get; set; }
}
