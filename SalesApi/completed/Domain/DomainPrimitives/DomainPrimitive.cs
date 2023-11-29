namespace SalesApi.Domain.DomainPrimitives;

public abstract class DomainPrimitive
{
    protected static bool EqualOperator(DomainPrimitive left, DomainPrimitive right)
    {
        if (left is null ^ right is null)
        {
            return false;
        }
        return left is null || left.Equals(right);
    }

    protected static bool NotEqualOperator(DomainPrimitive left, DomainPrimitive right)
    {
        return !EqualOperator(left, right);
    }

    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (DomainPrimitive)obj;

        return this.GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public static bool operator ==(DomainPrimitive one, DomainPrimitive two)
    {
        return EqualOperator(one, two);
    }

    public static bool operator !=(DomainPrimitive one, DomainPrimitive two)
    {
        return NotEqualOperator(one, two);
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }
}