namespace SalesClient.Client.Security;

/// <summary>
/// Service for authentication
/// </summary>
public interface IAuthenticationService
{
    void Login(string returnUrl = "/");

    void Logout();
}
