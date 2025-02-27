namespace SalesClient.Server;

public record UserInfo(
    bool IsAuthenticated,
    List<KeyValuePair<string, string>> Claims);