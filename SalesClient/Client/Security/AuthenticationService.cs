using Microsoft.AspNetCore.Components;
using System.Web;

namespace SalesClient.Client.Security;

public class AuthenticationService(NavigationManager navigationManager) : IAuthenticationService
{
    public void Login(string returnUrl = "/")
    {
        var url = $"client/account/Login?returnUrl={HttpUtility.UrlEncode(returnUrl)}";
        navigationManager.NavigateTo(url, forceLoad: true);
    }

    public void Logout()
    {
        navigationManager.NavigateTo("client/account/Logout", forceLoad: true);
    }
}