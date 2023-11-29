namespace SalesApi.Infrastructure;

public class UserPermissionRepository : IUserPermissionRepository
{
    private readonly Dictionary<string, string?> _userPermissions = new()
    {
        {"auth0|655c7e9a022f6b2083b15dc5", "no"},
        {"auth0|122c7e9a011f6b2083q175op", "no"},
        {"ozrjG9OAXgswPYYYmeQaDQZVPLDR3p9y@clients", "no"},
    };

    public Task<List<string>> GetUserMarketPermissions(string userId)
    {
        var marketId = _userPermissions.GetValueOrDefault(userId);
        //give market no to all uses even if not present in dictionary, to make testing more convenient
        if (marketId is null) return Task.FromResult(new List<string> { "no" });
        return Task.FromResult(new List<string> { marketId });
    }
}