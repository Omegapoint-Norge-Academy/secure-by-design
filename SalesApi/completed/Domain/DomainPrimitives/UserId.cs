using SalesApi.Domain.DomainPrimitives;

namespace SalesApi.Domain.Services;

public class UserId : DomainPrimitive
{
    public UserId(string subject)
    {
        AssertValidUserId(subject);

        Value = subject;
    }

    public string Value { get; }

    public static bool IsValidUserId(string subject)
    {
        return !string.IsNullOrEmpty(subject) && subject.Length < 50;
    }

    public static void AssertValidUserId(string subject)
    {
        if (!IsValidUserId(subject))
        {
            throw new DomainPrimitiveArgumentException<string>(subject);
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}