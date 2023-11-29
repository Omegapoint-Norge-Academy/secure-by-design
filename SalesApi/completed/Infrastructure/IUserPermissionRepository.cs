using SalesApi.Domain.DomainPrimitives;
using SalesApi.Domain.Services;

namespace SalesApi.Infrastructure;

public interface IUserPermissionRepository
{
    Task<List<MarketId>> GetUserMarketPermissions(UserId userId);
}