using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Tests.System;

public enum ProductScope
{
    Read,
    Write,
}

[TestFixture]
public class BaseTests
{
    private WebApplicationFactory<Program> _factory;
    protected HttpClient _client;
    protected HttpClient _extClient;
    protected IConfiguration _configuration;

    private string? _tokenUri;
    private string? _readClientId;
    private string? _readClientSecret;
    private string? _writeClientId;
    private string? _writeClientSecret;

    [SetUp]
    public void SetUp()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
        _extClient = new HttpClient();

        var builder = new ConfigurationBuilder()
            .AddJsonFile("testsettings.json", optional: false)
            .AddEnvironmentVariables();

        _configuration = builder.Build();

        _tokenUri = _configuration["TokenUri"];

        _readClientId = _configuration["ReadClient:ClientId"];
        _readClientSecret = _configuration["ReadClient:ClientSecret"];
        _writeClientId = _configuration["WriteClient:ClientId"];
        _writeClientSecret = _configuration["WriteClient:ClientSecret"];
    }

    protected async Task AuthorizeHttpClient(ProductScope scope)
    {
        string? token;
        if (scope == ProductScope.Read)
        {
            token = await GetAccessToken(scope);
        }
        else
        {
            token = await GetAccessToken(scope);
        }
        if (token is null)
        {
            throw new NullReferenceException("Did not receive proper access token from authentication API");
        }
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<string?> GetAccessToken(ProductScope scope)
    {
        var requestUrl = _tokenUri;

        var requestBody = new AuthRequestBody
        {
            ClientId = (scope == ProductScope.Read) ? _readClientId : _writeClientId,
            ClientSecret = (scope == ProductScope.Read) ? _readClientSecret : _writeClientSecret,
            Audience = "sales-api",
            GrantType = "client_credentials",
            Scope = (scope == ProductScope.Read) ? "products.read" : "products.write",
        };

        var body = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        var response = await _extClient.PostAsync(_tokenUri, body);

        response.EnsureSuccessStatusCode();

        TestContext.Out.WriteLine(response);

        var result = await response.Content.ReadFromJsonAsync<TokenResult>();

        return result?.access_token;
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private record TokenResult(string access_token);
}
