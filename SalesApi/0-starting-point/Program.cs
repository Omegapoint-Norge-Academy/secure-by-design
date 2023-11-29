using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using SalesApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Demo - Needed if we want all token claims from the IdP in the ClaimsPrincipal, 
// not filtered or transformed by ASP.NET Core default claim mapping
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddSingleton<IProductRepository, ProductRepository>();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton<IUserPermissionRepository, UserPermissionRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

