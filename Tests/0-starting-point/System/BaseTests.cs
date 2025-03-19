using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit.Abstractions;

namespace Tests.System;

public enum ProductScope
{
    Read,
    Write,
}

public class BaseTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    protected readonly HttpClient _client;

    private HttpClient _extClient;
    private string? _tokenUri;
    private string? _readClientId;
    private string? _readClientSecret;
    private string? _writeClientId;
    private string? _writeClientSecret;

    private readonly ITestOutputHelper _testOutput;

    public BaseTests(ITestOutputHelper testOutput)
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
        _extClient = new HttpClient();
        _testOutput = testOutput;

        var builder = new ConfigurationBuilder()
            .AddJsonFile("testsettings.json", optional: false)
            .AddEnvironmentVariables();

        var configuration = builder.Build();
        _tokenUri = configuration["TokenUri"];
        _readClientId = configuration["ReadClient:ClientId"];
        _readClientSecret = configuration["ReadClient:ClientSecret"];
        _writeClientId = configuration["WriteClient:ClientId"];
        _writeClientSecret = configuration["WriteClient:ClientSecret"];
    }

    protected async Task AuthorizeHttpClient(ProductScope scope)
    {
        var token = await GetAccessToken(scope);

        if (token is null)
            throw new NullReferenceException("Did not receive proper access token from authentication API");

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<string?> GetAccessToken(ProductScope scope)
    {
        var requestBody = new AuthRequestBody
        {
            ClientId = scope == ProductScope.Read ? _readClientId : _writeClientId,
            ClientSecret = scope == ProductScope.Read ? _readClientSecret : _writeClientSecret,
            Audience = "sales-api",
            GrantType = "client_credentials",
            Scope = scope == ProductScope.Read ? "products.read" : "products.write",
        };

        var body = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await _extClient.PostAsync(_tokenUri, body);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<TokenResult>();

        _testOutput.WriteLine(
            $"Status: {response.StatusCode}\nHeaders: {response.Headers}\nContent: {response.Content}");

        return result?.accessToken;
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private record TokenResult(
        [property: JsonPropertyName("access_token")]
        string accessToken
    );
}