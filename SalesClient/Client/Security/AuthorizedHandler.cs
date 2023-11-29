using System.Net;

namespace SalesClient.Client.Security;

public class AuthorizedHandler(CustomAuthenticationStateProvider authenticationStateProvider, IRedirectService redirectService) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);

        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();

        // If user is authenticated, execute request
        if (authState.User.Identity?.IsAuthenticated == true)
        {
            responseMessage = await base.SendAsync(request, cancellationToken);
        }

        // If user is not authenticated or server returned 401 Unauthorized, redirect to login page
        if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
        {
            redirectService.RedirectToLogin();
        }

        return responseMessage;
    }
}
