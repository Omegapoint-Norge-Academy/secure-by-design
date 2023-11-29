namespace SalesApi.Domain.DomainPrimitives;

public class ProductId
{
    public ProductId(string id)
    {
        AssertValid(id);
        Value = id;
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
            throw new ArgumentException(id);
        }
    }
}

