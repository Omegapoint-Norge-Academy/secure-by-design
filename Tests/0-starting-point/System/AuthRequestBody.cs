using System.Text.Json.Serialization;

namespace Tests.System;

public class AuthRequestBody
{
    [JsonPropertyName("client_id")]
    public string? ClientId { get; set; } = null!;
    [JsonPropertyName("client_secret")]
    public string? ClientSecret { get; set; } = null!;
    [JsonPropertyName("audience")]
    public string? Audience { get; set; } = null!;
    [JsonPropertyName("grant_type")]
    public string? GrantType { get; set; } = null!;
    [JsonPropertyName("scope")]
    public string? Scope { get; set; } = null!;
}
