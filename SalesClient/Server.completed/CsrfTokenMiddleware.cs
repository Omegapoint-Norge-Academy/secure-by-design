using System.Net;

namespace SalesClient.Server;

public class CsrfTokenMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("X-Csrf-Token", out _) &&
            !UrlIsWhitelisted(context) &&
            context.Request.Method != HttpMethod.Get.Method)
        {
            var response = context.Response;
            response.StatusCode = (int)HttpStatusCode.BadGateway;
            response.ContentType = "text/plain";

            await using var writer = new StreamWriter(response.Body);
            await writer.WriteAsync("CSRF");

            return;
        }

        await next(context);
    }

    private static bool UrlIsWhitelisted(HttpContext context)
    {
        string[] whiteListedUrls = ["/account/login", "/account/logout", "/signin-oidc"];
        return whiteListedUrls.Any(url => context.Request.Path.Equals(url));
    }
}