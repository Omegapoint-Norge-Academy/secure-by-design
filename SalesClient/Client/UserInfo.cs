namespace SalesClient.Client;

public record UserInfo(
    bool IsAuthenticated,
    List<KeyValuePair<string, string>> Claims);