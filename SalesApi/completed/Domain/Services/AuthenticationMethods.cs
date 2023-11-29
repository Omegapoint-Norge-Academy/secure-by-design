namespace SalesApi.Domain.Services;

[Flags]
public enum AuthenticationMethods
{
    None = 0,

    Unknown = 0x1,

    Password = 0x2,

    MFA = 0x4
}