using Blazored.Modal;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SalesClient.Client;
using SalesClient.Client.Security;
using SalesClient.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddAuthorizationCore(config =>
{
    config.AddPolicy("AuthenticatedUser", builder => builder.RequireAuthenticatedUser());
});
builder.Services.TryAddSingleton<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.TryAddSingleton(sp => (CustomAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());
builder.Services.AddTransient<AuthorizedHandler>();
builder.Services.AddTransient<CsrfTokenHandler>();

builder.Services.AddSingleton<IRedirectService, RedirectService>();
builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();

builder.Services.AddHttpClient<IUserInfoService, UserInfoService>(x => x.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<CsrfTokenHandler>();
builder.Services.AddHttpClient<IProductService, ProductService>(x => x.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<AuthorizedHandler>()
    .AddHttpMessageHandler<CsrfTokenHandler>();

builder.Services.AddBlazoredModal();

await builder.Build().RunAsync();
