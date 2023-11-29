namespace SalesApi.Domain.DomainPrimitives;

public class ProductId : DomainPrimitive
{
    public ProductId(string id)
    {
        AssertValid(id);
        Value = id;
    }

    public static ProductId Generate()
    {
        return new ProductId(Guid.NewGuid().ToString().Replace("-", string.Empty)[..8]);
    }

    public string Value { get; private set; }
    public static bool IsValid(string id)
    {
        return !string.IsNullOrEmpty(id) && id.Length < 10 && id.All(char.IsLetterOrDigit);
    }

    private static void AssertValid(string id)
    {
        if (!IsValid(id))
        {
            throw new DomainPrimitiveArgumentException<string>(id);
        }
    }

    public override string ToString() => Value.ToString();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

