using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Web;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
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
            var postLogoutUri = context.Properties.RedirectUri;
            if (!string.IsNullOrEmpty(postLogoutUri))
            {
                logoutUri += $"&returnTo={HttpUtility.UrlEncode(postLogoutUri)}";
            }

            context.ProtocolMessage.IssuerAddress = logoutUri;

            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization(options =>
{
    var defaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.AddPolicy("AuthenticatedUser", defaultPolicy);
    options.DefaultPolicy = defaultPolicy;
    options.FallbackPolicy = defaultPolicy;
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization("AuthenticatedUser");
app.MapReverseProxy().RequireAuthorization("AuthenticatedUser");

app.MapFallbackToFile("index.html").AllowAnonymous();

app.Run();
