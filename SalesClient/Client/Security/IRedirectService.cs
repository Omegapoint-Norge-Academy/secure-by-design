namespace SalesClient.Client.Security;

public interface IRedirectService
{
    void RedirectToLogin(string? returnUrl = null);

    void RedirectAccessDenied();
}