namespace SalesApi.Domain.DomainPrimitives;

public class ProductName : DomainPrimitive
{
    public ProductName(string name)
    {
        AssertValid(name);
        Value = name;
    }

    public string Value { get; }

    public static bool IsValid(string name)
    {
        // Names are very hard to restrict, but we at least limit the size.
        // See also https://stackoverflow.com/q/20958
        return
            !string.IsNullOrEmpty(name) &&
            name.Length <= 200;
    }

    private static void AssertValid(string name)
    {
        if (!IsValid(name))
        {
            throw new DomainPrimitiveArgumentException<string>(name);
        }
    }

    public override string ToString() => Value.ToString();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}