using IdentityModel.AspNetCore.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using SalesApi;
using SalesApi.Domain.Services;
using SalesApi.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Demo - Needed if we want all token claims from the IdP in the ClaimsPrincipal, 
// not filtered or transformed by ASP.NET Core default claim mapping
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

// Demo - This JWT middleware has secure defaults, with validation according to the JWT spec, 
// all we need to do is configure iss and aud, and add valid type.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        builder.Configuration.Bind("JwtBearerOptions", options);
        // TokenValidationParameters not not currently supported in appsettings for .NET 8
        // Note that type validation might differ, depending on token serivce (IdP).
        // If possible validate that that the token is an access token by using "at+jwt"
        options.TokenValidationParameters.ValidTypes = new[] { "JWT" };
    });

// Demo - Require Bearer authentication scheme for all requests (including non mvc requests), 
// even if no other policy has been configured.
// Enable public endpoints by decorating with the AllowAnonymous attribute.
builder.Services.AddAuthorization(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .Build();

    options.DefaultPolicy = policy;
    options.FallbackPolicy = policy;

    // Even if we validate permission to perform the operation in the domain layer,
    // we should also verify this basic access as early as possible, e g by using ASP.NET Core policies.
    // This could also be done in a API-gateway in front of us, but the core domain should not 
    // assume any of this. Defence in depth and Zero trust!
    options.AddPolicy(ClaimSettings.ProductsRead, policy => policy.RequireScope(ClaimSettings.ProductsRead));
    options.AddPolicy(ClaimSettings.ProductsWrite, policy => policy.RequireScope(ClaimSettings.ProductsWrite));
});




builder.Services.AddSingleton<IProductRepository, ProductRepository>();
builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddSingleton<IUserPermissionRepository, UserPermissionRepository>();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddHttpContextAccessor();
builder.Services.AddPermissionService();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
