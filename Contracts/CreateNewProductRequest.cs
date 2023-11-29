namespace Contracts;

public record CreateNewProductRequest(string Name, string Category, string Manufacturer, decimal Price, string Currency, string MarketId);