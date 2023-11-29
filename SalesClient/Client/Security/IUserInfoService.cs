
namespace SalesClient.Client.Security;

public interface IUserInfoService
{
    Task<UserInfo> GetUserInfo();
}
