using SalesApi.Domain.DomainPrimitives;

namespace SalesApi.Domain.Services;

public interface IPermissionService
{
    bool CanReadProducts { get; }

    bool CanWriteProducts { get; }

    bool CanDoHighPrivilegeOperations { get; }

    // MarketId MarketId { get; }

    UserId? UserId { get; }

    ClientId? ClientId { get; }

    AuthenticationMethods AuthenticationMethods { get; }

    bool HasPermissionToMarket(MarketId requestedMarket);
}
