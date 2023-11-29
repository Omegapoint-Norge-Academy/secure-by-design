namespace SalesApi.Infrastructure;

public interface IUserPermissionRepository
{
    Task<List<string>> GetUserMarketPermissions(string userId);
}