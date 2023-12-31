using System.Runtime.Serialization;

namespace SalesApi.Domain;

[Serializable]
public class DomainPrimitiveArgumentException<T> : ArgumentException
{
    public DomainPrimitiveArgumentException()
    {
    }

    public DomainPrimitiveArgumentException(T value) : base($"The value {value} is not valid.")
    {
    }

    public DomainPrimitiveArgumentException(string message, T value) : base(message)
    {
        Value = value;
    }

    public T? Value { get; }

    protected DomainPrimitiveArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        Value = (T?)info.GetValue(nameof(Value), typeof(T));
    }

    [Obsolete]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        base.GetObjectData(info, context);

        info.AddValue(nameof(Value), Value);
    }
}