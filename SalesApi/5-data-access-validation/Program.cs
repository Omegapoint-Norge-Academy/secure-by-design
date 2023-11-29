using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using SalesApi.Infrastructure;

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
});

builder.Services.AddSingleton<IUserPermissionRepository, UserPermissionRepository>();
builder.Services.AddSingleton<IProductRepository, ProductRepository>();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddSingleton<IClaimsTransformation, ClaimsTransformation>();

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

