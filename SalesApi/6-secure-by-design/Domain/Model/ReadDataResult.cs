namespace SalesApi.Domain.Model;
public enum ReadDataResult
{
    Success = 0,
    NotFound,
    NoAccessToData,
    NoAccessToOperation,
    InvalidData
}