using SalesApi.Domain.DomainPrimitives;

namespace SalesApi.Domain.Model;

public class Product(ProductId? id, ProductName name, Money price, MarketId marketId)
{
    public ProductId Id { get; private set; } = id ?? ProductId.Generate();
    public ProductName Name { get; private set; } = name;
    public Money Price { get; private set; } = price;
    public MarketId MarketId { get; private set; } = marketId;

    public (WriteDataResult, Product?) UpdatePrice(Money newPrice)
    {
        if (newPrice.Currency != Price.Currency) return (WriteDataResult.InvalidDomainOperation, null);

        Price = newPrice;
        return (WriteDataResult.Success, this);
    }
}