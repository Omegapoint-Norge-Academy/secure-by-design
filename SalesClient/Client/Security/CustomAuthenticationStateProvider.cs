using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Components.Authorization;

namespace SalesClient.Client.Security;

public class CustomAuthenticationStateProvider(IUserInfoService userInfoService) : AuthenticationStateProvider
{
    private readonly TimeSpan _userCacheRefreshInterval = TimeSpan.FromSeconds(60);
    private DateTimeOffset _userLastCheck = DateTimeOffset.FromUnixTimeSeconds(0);
    private ClaimsPrincipal? _cachedUser;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var now = DateTimeOffset.Now;
        if (now < _userLastCheck + _userCacheRefreshInterval && _cachedUser != null)
        {
            return new AuthenticationState(_cachedUser);
        }

        _cachedUser = await FetchUser();
        _userLastCheck = now;
        var authState = new AuthenticationState(_cachedUser);

        if (_cachedUser.Identity?.IsAuthenticated == true)
        {
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        return authState;
    }

    private async Task<ClaimsPrincipal> FetchUser()
    {
        var userInfo = await userInfoService.GetUserInfo();

        if (userInfo?.IsAuthenticated != true)
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }

        var claims = userInfo.Claims?.Select(x => new Claim(x.Key, x.Value)) ?? new List<Claim>();

        var identity = new ClaimsIdentity(claims, "Server authentication", JwtClaimTypes.Name, JwtClaimTypes.Role);
        var user = new ClaimsPrincipal(identity);
        return user;
    }
}