# Workshop guide - Secure Client

- [Introduction](#introduction)
- [Part 0](#part-0)
- [Part 1 - Login and Logout](#part-1---login-and-logout)
  - [Dependencies](#dependencies)
  - [Bootstrapping](#bootstrapping)
    - [Middleware](#middleware)
    - [Add Authentication](#add-authentication)
      - [Cookie options](#cookie-options)
      - [OpenID connect options](#openid-connect-options)
      - [Default schemas](#default-schemas)
    - [Add Authorization](#add-authorization)
  - [Account controller](#account-controller)
  - [Part 1 milestone: Test login](#part-1-milestone--test-login)
- [Part 2 - User context](#part-2---user-context)
  - [User controller](#user-controller)
  - [Part 2 milestone: Test user context](#part-2-milestone--test-user-context)
- [Part 3 - Accessing remote API](#part-3---accessing-remote-api)
  - [Bootstrapping](#bootstrapping-1)
  - [Exchanging cookie for access token](#exchanging-cookie-for-access-token)
  - [Part 3 milestone: Test API access](#part-3-milestone--test-api-access)
- [Part 4 - Refreshing the token](#part-4---refreshing-the-token)
  - [Dependencies](#dependencies)
  - [Create a Token Handler](#create-a-token-handler)
  - [Add offline access scope](#add-offline-access-scope)
  - [Modify Yarp request transform](#modify-yarp-request-transform)
  - [Part 4 milestone: Test refresh token](#part-4-milestone--test-refresh-token)
- [Appendix](#appendix)
  - [Debugging .NET with Fiddler](#debugging-net-with-fiddler)
    - [HTTPS](#https)
    - [Capture .NET traffic](#capture-net-traffic)
    - [Use filters](#use-filters)
  - [References](#references)

# Introduction

This part of the course will guide you through how you can create a secure client using C# and dotnet 8. We use the OAuth2 and OpenID Connect standards and the backend for frontend (BFF) pattern.

The content is divided into four parts. Step one is by far the most work intensive

# Part 0

Start by going to the [Server.0-starting-point](Server.0-starting-point) project. This is the project you will start from.

To run the solution two parts need to run:

- Run the SalesApi in its completed version in the background
- To start the client including the BFF, just run the SalesClient/Server project. This will also start the frontend.

# Part 1 - Login and Logout

## Dependencies

Install the following dependencies using Nuget

- IdentityModel
- Microsoft.AspNetCore.Authentication.OpenIdConnect

## Bootstrapping

### Middleware

Add authentication and authorization middleware to program file.
It should be added after `UseRouting()` but before `MapControllers()`.

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

Authentication Middleware `UseAuthentication()` attempts to authenticate the user before they're allowed access to secure resources.
Authorization Middleware `UseAuthorization()` authorizes a user to access secure resources.

### Add Authentication

There needs to be two authentication schemes, one for cookie, and one for OpenID Connect.
These name of these schemes are embedded in framework constants:

```csharp
CookieAuthenticationDefaults.AuthenticationScheme
OpenIdConnectDefaults.AuthenticationScheme
```

These will be configured separately by adding them us below

```csharp
builder.Services
    .AddAuthentication()
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
    })
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
    });
```

Add this to `Program.cs` just after var `builder = WebApplication.CreateBuilder(args);`

Both these schemes have to be configured. Lets focus on the cookie first.

#### Cookie options

This cookie defines the session between the frontend and the BFF.

These are the options that needs to be configured:

- [Cookie.HttpOnly](https://cheatsheetseries.owasp.org/cheatsheets/Session_Management_Cheat_Sheet.html#httponly-attribute)
- [Cookie.SecurePolicy](https://cheatsheetseries.owasp.org/cheatsheets/Session_Management_Cheat_Sheet.html#secure-attribute)
- [Cookie.SameSite](https://cheatsheetseries.owasp.org/cheatsheets/Session_Management_Cheat_Sheet.html#samesite-attribute)

To manage the lifetime of the cookie configure these properties:

- ExpireTimeSpan
- SlidingExpiration

Set the expire to 60 minutes, and sliding to true.

The SlidingExpiration is set to true to instruct the handler to re-issue a new cookie with a new expiration time any time it processes a request which is more than halfway through the expiration window.

A session cookie should always be considered essential to the application.
To skip cookie consent, configure `IsEssential` to `true`.

Lastly add code to deliver 403 forbidden instead of redirecting to the login page when a resource is denied.

```csharp
options.Events.OnRedirectToAccessDenied = context =>
{
    context.Response.StatusCode = StatusCodes.Status403Forbidden;
    return Task.CompletedTask;
};
```

When all this is done, the code should look like this:

<details>
<summary><b>Spoiler (Full code)</b></summary>
<p>

```csharp
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.IsEssential = true;

    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;

    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
})
```

</p>
</details>

#### OpenID connect options

This is the configuration that allows us to login using the identity provider (IDP).

Add options for using Authorization code flow with PKCE:

```csharp
options.ResponseType = OpenIdConnectResponseType.Code;
options.UsePkce = true;
```

Add additional options

```csharp
options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
options.RequireHttpsMetadata = true;
options.SaveTokens = true;
```

The `SignInScheme` will make sure that the result of the sign in will be saved to the cookie.
Setting `RequireHttpsMetadata` to true will make sure https is used when fetching metadata from the IDP.
`SaveTokens` make sure tokens are saved to the cookie for later use.

Add IDP configuration (client secret will be handed out separately)

```csharp
options.Authority = "https://omegapoint-norge-workshop.eu.auth0.com";
options.ClientId = "sELZRGRZpJSJr2VIdsiD0XQZRW6T5nCN";
options.ClientSecret = "";
```

Make sure to configure what scopes to request:

```csharp
options.Scope.Clear();
options.Scope.Add("openid");
options.Scope.Add("profile");
```

The `openid` scope is added to instruct that we are using OpenID connect and not pure OAuth2.0.
The `profile` scope will instruct the IDP to return claims as
`name`, `family_name`, `given_name`, `middle_name`, `nickname`, `picture`, and `updated_at` if available.
In essence the `profile` scope lets us get basic user profile info.

We also want to make sure that redirect to the identity provider is not done when calling the API, or the user info endpoint
The framework will try to redirect to the identity provider when the API responds with Unauthorized.
We can modify this behaviour by adding intercepting the `OnRedirectToIdentityProvider` event and setting Unauthorized as the response.

```csharp
options.Events.OnRedirectToIdentityProvider = context =>
{
    if (context.Request.Path.StartsWithSegments("/api") ||
        context.Request.Path.StartsWithSegments("/client/user"))
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.HandleResponse();
    }

    return Task.CompletedTask;
};
```

Most identity providers (IDPs) have there own quirks.
Auth0 have a sign out endpoint that does not conform to standards.
To handle this, add this code to the options:

```csharp
options.Events.OnRedirectToIdentityProviderForSignOut = context =>
{
    var logoutUri = $"{context.Options.Authority}/v2/logout?client_id={context.Options.ClientId}";
    var postLogoutUri = context.ProtocolMessage.RedirectUri;
    if (!string.IsNullOrEmpty(postLogoutUri))
    {
        logoutUri += $"&returnTo={HttpUtility.UrlEncode(postLogoutUri)}";
    }

    context.ProtocolMessage.IssuerAddress = logoutUri;

    return Task.CompletedTask;
};
```

This code builds the sign out request uri and configures the sign out callback.

When all this is added, the code should look like this:

<details>
<summary><b>Spoiler (Full code)</b></summary>
<p>

```csharp
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.ResponseType = OpenIdConnectResponseType.Code;
    options.UsePkce = true;

    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.RequireHttpsMetadata = true;
    options.SaveTokens = true;

    options.Authority = "https://omegapoint-norge-workshop.eu.auth0.com";
    options.ClientId = "sELZRGRZpJSJr2VIdsiD0XQZRW6T5nCN";
    options.ClientSecret = "";

    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");

    options.Events.OnRedirectToIdentityProvider = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api") ||
            context.Request.Path.StartsWithSegments("/client/user"))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.HandleResponse();
        }

        return Task.CompletedTask;
    };

    // Auth0 specific implementation
    options.Events.OnRedirectToIdentityProviderForSignOut = context =>
    {
        var logoutUri = $"{context.Options.Authority}/v2/logout?client_id={context.Options.ClientId}";
        var postLogoutUri = context.ProtocolMessage.RedirectUri;
        if (!string.IsNullOrEmpty(postLogoutUri))
        {
            logoutUri += $"&returnTo={HttpUtility.UrlEncode(postLogoutUri)}";
        }

        context.ProtocolMessage.IssuerAddress = logoutUri;

        return Task.CompletedTask;
    };
})
```

</p>
</details>

#### Default schemas

We need to tell .NET that our default scheme is cookie, and that our challenge scheme is openid.
To do this, add some options to `AddAuthentication()`

```csharp
.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
```

### Add Authorization

We need to add some kind of policy for our users.
Lets add authorization `AddAuthorization()` and create a simple policy.
The policy should only require a user to be authenticated.
Hint: use the `AuthorizationPolicyBuilder()`.

Make sure the policy is configured as default policy and fallback policy

The code should look something like this:

<details>
<summary><b>Spoiler (Full code)</b></summary>
<p>

```csharp
builder.Services.AddAuthorization(options =>
{
    var defaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.AddPolicy("AuthenticatedUser", defaultPolicy);
    options.DefaultPolicy = defaultPolicy;
    options.FallbackPolicy = defaultPolicy;
});
```

</p>
</details>

Also add the policy to all controllers and the reverse proxy by adding `RequireAuthorization()` to the middlewares.
Make sure that the policy names id the same as used in `AddPolicy()` earlier.

```csharp
app.MapControllers().RequireAuthorization("AuthenticatedUser");
app.MapReverseProxy().RequireAuthorization("AuthenticatedUser");
```

When this is added, you dont need to add the authorize attribute to all controllers.

## Account controller

The account controller should handle login and logout.

Create a controller called `AccountController`,
and add two endpoints:

**client/account/login:** This endpoint should be a http get, and it should accept a `returnUrl` as a query parameter.
It should return a `Challenge()` where the `returnUrl` is passed inn by `AuthenticationProperties`. If the `returnUrl` is null, then it should be set to "/".
The `returnUrl` should be validated to be relative. This will protect against open redirector attacks. Use `Url.IsLocalUrl` to check if the `returnUrl` is valid.
See https://learn.microsoft.com/en-us/aspnet/core/security/preventing-open-redirects?view=aspnetcore-7.0 for more info.
The endpoint should be accessible for anonymous users.

**client/account/logout:** This endpoint should be a http get, and accept no parameters.
It should do a `HttpContext.SignOutAsync()` on both the cookie scheme, and the openid scheme.
When signing out of the cookie scheme a redirect uri to the home page of the app should be configured.

<details>
<summary><b>Spoiler (Full code)</b></summary>
<p>

```csharp
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
            RedirectUri = "https://localhost:7110/",
        });
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
```

</p>
</details>

## Part 1 milestone: Test login

This is a milestone, login and logout could now be tested.
When logging in, you should be redirected to Auth0 login page.
When prompted with login from Auth0, sign up with an email and password of your choice.
To verify that you are actually logged in, go to the browser console and check if a cookie named `.AspNetCore.Cookies` is present.
Also try logging out, and logging in again and verify that you are prompted with login again.

If anything fails, it should be fixed before moving on.

Debugging tips:

- Use fiddler to inspect the communication between the BFF and the IDP. See [appendix](#debugging-net-with-fiddler)
- Use browser tools and inspect the console and network.
- Compare with [solution](Server.1-login-and-logout)

# Part 2 - User context

This parts is about adding an authentication context to the frontend.

## User controller

The user controller should return authentication state. This state is intended for the client.
Create a user info record like below:

```csharp
public record UserInfo(
    bool IsAuthenticated,
    List<KeyValuePair<string, string>> Claims);
```

Create a user controller with the current endpoint:

**client/user:** This endpoint should be a http get, and accept no parameters.
It should return a `UserInfo` record. `UserInfo` should be populated with data from the `UserPrincipal`.
The `UserPrincipal` can be accessed with the property `User` from the `ControllerBase`. This object contains authentication states and all claims.
When adding claims to `UserInfo`, make sure to only add the claims we need.
We do not want to expose all claims for security reasons.
For now, expose only the claim named `name`.

<details>
<summary><b>Spoiler (Full code)</b></summary>
<p>

```csharp
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
```

</p>
</details>

## Part 2 milestone: Test user context

This is a milestone, user context can now be tested. Verify that the logout/login in the client does show the correct state, and that you are able to log in and out. If anything fails, it should be fixed before moving on.

Debugging tips:

- Use fiddler to inspect the communication between the BFF and the IDP. See [appendix](#debugging-net-with-fiddler)
- Use browser tools and inspect the console and network.
- Compare with [solution](Server.2-user-context)

When prompted with login from Auth0, sign up with an email and password of your choice.

# Part 3 - Accessing remote API

We will now connect to the SalesApi.

- Base uri: https://localhost:7094
- Audience: `sales-api`
- Scope: `products.read products.write`

## Bootstrapping

To get an access token for the API we need to request the API scope.
Add the scope to the options in `AddOpenIdConnect`.

Auth0 also requires that audience is specified when requesting scope for an API.
This is not always required by all IDPs. The `OnRedirectToIdentityProvider` event allows us to add a property with the audience of the API.
Add the following code to the event:

```csharp
context.ProtocolMessage.SetParameter("audience", "sales-api");
```

Also request relevant scopes for the remote API

```csharp
options.Scope.Add("products.read");
options.Scope.Add("products.write");
```

## Exchanging cookie for access token

The access token is located is accessible through the HttpContext. We are using a reverse proxy for all API requests.
We can configure a transform on the proxy that adds the access token to the request.

```csharp
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(builderContext =>
    {
        builderContext.AddRequestTransform(async transformContext =>
        {
            var accessToken = await transformContext.HttpContext.GetTokenAsync("access_token");
            if (accessToken != null)
            {
                transformContext.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        });
    });
```

## Part 3 milestone: Test API access

Test that access we now can access the products when we are logged in.
If anything fails, it should be fixed before moving on.

Debugging tips:

- Set breakpoint inside `AddRequestTransform` and inspect the access token using https://jwt.io
- Compare with [solution](Server.3-accessing-remote-api)

# Part 4 - Refreshing the token

The access token is configured to expire in 60 seconds. Currently the users will have to log in and out to get a new access token. Let's start using refresh tokens to improve the user experience.

## Dependencies

We will use Duende Software's open source token management package. Most of Duende is licenced, as of 27.02.2025 the Automatic token management packaged is released under the Apache 2.0 license. Keep in mind that this can change.

Install the following dependencies using Nuget

- Duende.AccessTokenManagement.OpenIdConnect

## Create a Token Handler

Start with creating a token handler that uses Duende's `OpenIdConnectUserAccessTokenHandler` to fetch tokens.

```csharp
public class AppTokenHandler(
    IDPoPProofService dPoPProofService,
    IDPoPNonceStore dPoPNonceStore,
    IUserAccessor userAccessor,
    IUserTokenManagementService userTokenManagement,
    ILogger<OpenIdConnectClientAccessTokenHandler> logger,
    UserTokenRequestParameters? parameters = null)
    : OpenIdConnectUserAccessTokenHandler(dPoPProofService, dPoPNonceStore, userAccessor, userTokenManagement, logger, parameters)
{
    public new Task<ClientCredentialsToken> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        return base.GetAccessTokenAsync(false, cancellationToken);
    }
}
```

Add the following registrations to `Program.cs`

```csharp
builder.Services.AddOpenIdConnectAccessTokenManagement();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddTransient<OpenIdConnectUserAccessTokenHandler>();
builder.Services.AddTransient<AppTokenHandler>();
```

The `OpenIdConnectUserAccessTokenHandler` from Duenede taks care of token management by inspecting the expiration of the access token. If the access token is expierer it uses the refresh token to aquire a new access token.

## Add offline access scope

OpenID Connect has a scope named `offline_access`. This has to be requested by the application to enable refresh tokens

```csharp
options.Scope.Add("offline_access");
```

## Modify Yarp request transform

Modify the request transformation from step 3 to get tokens using the new `AppTokenHandler` instead of getting it directly from the http context.

Use `var tokenHandler = transformContext.HttpContext.RequestServices.GetRequiredService<AppTokenHandler>();` to get the token handler.

<details>
<summary><b>Spoiler (Full code)</b></summary>
<p>

```csharp
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(builderContext =>
    {
        builderContext.AddRequestTransform(async transformContext =>
        {
            var tokenHandler = transformContext.HttpContext.RequestServices.GetRequiredService<AppTokenHandler>();
            var accessToken = (await tokenHandler.GetAccessTokenAsync()).AccessToken;
            if (accessToken != null)
            {
                transformContext.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        });
    });
```

</p>
</details>

## Part 4 milestone: Test refresh token

Test that we still have access to products in the web page after 60 secons have passed

# Appendix

## Debugging .NET with Fiddler

### HTTPS

To view https traffic in fiddler, go to **Tools -> Fiddler Options -> HTTPS** and activate HTTPS by checking the boxes shown below
![alt text](../Resources/fiddler_https.PNG?raw=true)

### Capture .NET traffic

Fiddler relies on proxies to intercept requests. To inspect all traffic from .NET a proxy must be added.
Open `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Config\machine.config` and add the following code section at the bottom immediately after `</system.web>`

```xml
<system.net>
    <defaultProxy enabled = "true" useDefaultCredentials = "true">
        <proxy autoDetect="false" bypassonlocal="false" proxyaddress="http://127.0.0.1:8888" usesystemdefault="false" />
    </defaultProxy>
</system.net>
```

This will allow fiddler to read .NET traffic.

**NB:** remember to remove the proxy when finished.

### Use filters

Use filters to not get overloaded with traffic that is not interesting.

Recommended host filters for this application: `dev-my7g8x3rrwfzi3lh.eu.auth0.com; localhost:5001; localhost:44469;`

![alt text](../Resources/fiddler_filter.PNG?raw=true)

## References

- Value object as C# records? [Probably not.](https://enterprisecraftsmanship.com/posts/csharp-records-value-objects/)
- [Functional C#: Handling failures, input errors](https://enterprisecraftsmanship.com/posts/functional-c-handling-failures-input-errors/)
