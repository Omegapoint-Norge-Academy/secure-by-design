namespace SalesApi.Domain.DomainPrimitives;
public class Amount : DomainPrimitive
{
    public double Value { get; private set; }

    public Amount(double value)
    {
        AssertValid(value);
        Value = value;
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static Amount operator +(Amount a1, Amount a2)
    {
        return new Amount(a1.Value + a2.Value);
    }
    public static Amount operator -(Amount a1, Amount a2)
    {
        return new Amount(a1.Value - a2.Value);
    }

    public override string ToString()
    {
        return $"{Math.Round(Value, 2)}";
    }

    private static void AssertValid(double value)
    {
        if (!IsValid(value))
        {
            throw new DomainPrimitiveArgumentException<double>(value);
        }
    }

    public static bool IsValid(double obj)
    {
        if (obj < 0)
        {
            return false;
        }
        return true;
    }
}