using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SalesClient.Server.Controllers;

[ApiController]
[Route("client/[controller]")]
public class AccountController : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("Login")]
    public ActionResult Login([FromQuery] string? returnUrl)
    {
        if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
        {
            returnUrl = "/";
        }
        var properties = new AuthenticationProperties { RedirectUri = $"https://localhost:7110{returnUrl}" };

        return Challenge(properties);
    }

    [Authorize]
    [HttpGet("Logout")]
    public async Task Logout()
    {
        await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
        {
            RedirectUri = "https://localhost:7110",
        });
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}