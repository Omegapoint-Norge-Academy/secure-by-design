using Duende.AccessTokenManagement;
using Duende.AccessTokenManagement.OpenIdConnect;

namespace SalesClient.Server;

public class AppTokenHandler(
    IDPoPProofService dPoPProofService,
    IDPoPNonceStore dPoPNonceStore,
    IUserAccessor userAccessor,
    IUserTokenManagementService userTokenManagement,
    ILogger<OpenIdConnectClientAccessTokenHandler> logger,
    UserTokenRequestParameters? parameters = null)
    : OpenIdConnectUserAccessTokenHandler(dPoPProofService, dPoPNonceStore, userAccessor, userTokenManagement, logger,
        parameters)
{
    public Task<ClientCredentialsToken> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        return base.GetAccessTokenAsync(false, cancellationToken);
    }
}