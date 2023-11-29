namespace SalesApi.Domain.Model;
public enum WriteDataResult
{
    Success = 0,
    NotFound,
    NoAccessToData,
    NoAccessToOperation,
    InvalidData,
    InvalidDomainOperation
}