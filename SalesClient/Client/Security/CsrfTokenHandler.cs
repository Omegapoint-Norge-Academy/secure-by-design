namespace SalesClient.Client.Security;

public class CsrfTokenHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.Headers.Add("X-Csrf-Token", "1");
        return base.SendAsync(request, cancellationToken);
    }
}