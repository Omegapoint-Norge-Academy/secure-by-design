
namespace SalesApi.Domain.DomainPrimitives;

public class MarketId : DomainPrimitive
{
    public MarketId(string id)
    {
        AssertValidId(id);

        Value = id;
    }

    public string Value { get; }

    public static bool IsValid(string id)
    {
        var allowList = new[] { "se", "no", "fi" };

        return allowList.Contains(id, StringComparer.OrdinalIgnoreCase);
    }

    public static void AssertValidId(string id)
    {
        if (!IsValid(id))
        {
            throw new DomainPrimitiveArgumentException<string>(id);
        }
    }

    public override string ToString()
    {
        return Value;
    }


    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

}
