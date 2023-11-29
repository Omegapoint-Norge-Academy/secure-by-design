using Microsoft.AspNetCore.Components;
using System.Web;

namespace SalesClient.Client.Security;

public class RedirectService(NavigationManager navigationManager) : IRedirectService
{
    public void RedirectToLogin(string? returnUrl = null)
    {
        if (string.IsNullOrEmpty(returnUrl))
        {
            returnUrl = navigationManager.Uri;
        }

        var url = $"/login?redirectUrl={HttpUtility.UrlEncode(returnUrl)}";

        navigationManager.NavigateTo(url, forceLoad: true);
    }

    public void RedirectAccessDenied()
    {
        navigationManager.NavigateTo("/accessDenied", forceLoad: true);
    }
}
