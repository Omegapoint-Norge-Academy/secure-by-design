using Microsoft.AspNetCore.Mvc;
using SalesClient.Client;

namespace SalesClient.Server.Controllers;

[ApiController]
[Route("client/[controller]")]
public class UserController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(UserInfo), StatusCodes.Status200OK)]
    public IActionResult GetCurrentUser()
    {
        var claimsToExpose = new List<string>()
        {
            "name"
        };

        var user = new UserInfo(
            User.Identity?.IsAuthenticated ?? false,
            User.Claims
                .Select(c => new KeyValuePair<string, string>(c.Type, c.Value))
                .Where(c => claimsToExpose.Contains(c.Key))
                .ToList());
        return Ok(user);
    }
}