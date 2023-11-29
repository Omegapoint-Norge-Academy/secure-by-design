
namespace SalesApi.Domain.DomainPrimitives;

public class ClientId : DomainPrimitive
{
    public ClientId(string subject)
    {
        AssertValidClientId(subject);

        Value = subject;
    }

    public string Value { get; }

    public static bool IsValidClientId(string clientId)
    {
        return !string.IsNullOrEmpty(clientId) && clientId.Length < 50;
    }

    public static void AssertValidClientId(string clientId)
    {
        if (!IsValidClientId(clientId))
        {
            throw new DomainPrimitiveArgumentException<string>(clientId);
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}