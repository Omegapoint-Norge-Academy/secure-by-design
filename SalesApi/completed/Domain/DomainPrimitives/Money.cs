using System.Globalization;
using System.Text.Json.Serialization;

namespace SalesApi.Domain.DomainPrimitives;

public class Money : DomainPrimitive, IComparable<Money>
{
    private static readonly string[] _supportedCurrencies = ["NOK", "USD", "EUR"];
    public string Currency { get; private set; }
    public decimal Value { get; private set; }


    [JsonConstructor]
    public Money(decimal value, string currency)
    {
        AssertValid(value, currency);
        Value = value;
        Currency = currency;
    }

    public bool IsZero => Value == 0;

    public static bool IsValid(decimal value, string currency)
    {
        if (value < 0) return false;
        if (currency == null) return false;
        if (!_supportedCurrencies.Contains(currency)) return false;
        return true;
    }

    private static void AssertValid(decimal value, string currency)
    {
        if (!IsValid(value, currency))
        {
            throw new DomainPrimitiveArgumentException<decimal>(value);
        }
    }

    public static Money operator *(Money money, Amount amount)
    {
        return new Money(money.Value * new decimal(amount.Value), money.Currency);
    }

    public static Money operator +(Money m1, Money m2)
    {
        if (m1.Currency != m2.Currency) throw new ArgumentException("Cannot add different currencies");
        return new Money(m1.Value + m2.Value, m1.Currency);
    }

    public static Money operator -(Money m1, Money m2)
    {
        if (m1.Currency != m2.Currency) throw new ArgumentException("Cannot subtract different currencies");
        return new Money(m1.Value - m2.Value, m1.Currency);
    }


    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return Currency;
    }

    public override string ToString()
    {
        return $"{Math.Round(Value, 2)} {Currency.ToString()}";
    }

    public int CompareTo(Money? other)
    {
        if (other is null) return 1;
        if (Currency != other.Currency) throw new ArgumentException("Cannot compare different currencies");
        return Value.CompareTo(other.Value);
    }
}