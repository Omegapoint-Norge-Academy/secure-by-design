using SalesApi.Domain.DomainPrimitives;

namespace SalesApi.Domain.Services;

public interface IPermissionService
{
    bool CanReadProducts { get; }

    bool CanWriteProducts { get; }

    string MarketId { get; }

    string? UserId { get; }

    bool HasPermissionToMarket(string requestedMarket);
}
