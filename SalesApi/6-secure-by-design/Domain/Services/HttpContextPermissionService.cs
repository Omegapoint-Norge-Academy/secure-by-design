using SalesApi.Domain.DomainPrimitives;
using SalesApi.Infrastructure;
using System.Security.Claims;

namespace SalesApi.Domain.Services;

public class HttpContextPermissionService : IPermissionService
{
    // There is a balance between the PermissionService and the ClaimsTransformer.
    // In our case we have moved all code from the ClaimsTransformer to this class,
    // to keep all permission logic in this class, independent of other componentes.
    // Thus, it is the Permission service responsibility to incapsulate token specific 
    // details like custom scope values etc, so we don´t get dependencies to token 
    // formats and protocol details spread out in our code.
    //
    // But note that depending on token format there are cases where we might want 
    // to use a ClaimsTransformer in addition to this class.
    //
    // Also note that in this repo we have placed the PermissionService in our bussiness
    // domain, for other scenarios it might be more appropriate to move this to a
    // subdomain etc. The important thing is that the ProductService requires
    // complete access control, that this is a mandatory part of our core bussiness domain.
    // Also note that this pattern enbles the use of an external policy or permission service,
    // which might do caching etc.   

    public HttpContextPermissionService(IHttpContextAccessor contextAccessor, IUserPermissionRepository userPermissionRepository)
    {
        var principal = contextAccessor.HttpContext?.User;

        if (principal == null)
        {
            if (contextAccessor.HttpContext == null)
            {
                throw new ArgumentException("HTTP Context is null", nameof(contextAccessor));
            }

            throw new ArgumentException("User object is null", nameof(contextAccessor));
        }

        UserId = principal.FindFirstValue("sub");

        // It is important to honor any scope that affect our domain
        IfScope(principal, "products.read", () => CanReadProducts = true);
        IfScope(principal, "products.write", () => CanWriteProducts = true);

        if (UserId is not null)
        {
            var userPermissions = userPermissionRepository.GetUserMarketPermissions(UserId).GetAwaiter().GetResult();
            MarketId = userPermissions.FirstOrDefault();
        }
    }

    public bool CanReadProducts { get; private set; }

    public bool CanWriteProducts { get; private set; }

    public string? MarketId { get; private set; }

    public string? UserId { get; private set; }

    public string? ClientId { get; private set; }

    public bool HasPermissionToMarket(string requestedMarket)
    {
        if (MarketId is null) return false;
        return string.Equals(MarketId, requestedMarket, StringComparison.OrdinalIgnoreCase);
    }

    private static void IfScope(ClaimsPrincipal principal, string scope, Action action)
    {
        //Scopes can be a space separated list of scopes, so we need to split and check each scope
        if (principal.Claims.Any(claim => claim.Type == "scope" && claim.Value.Split(' ').Contains(scope)))
        {
            action();
        }

        if (principal.HasClaim(claim => claim.Type == "scope" && claim.Value == scope))
        {
            action();
        }
    }
}