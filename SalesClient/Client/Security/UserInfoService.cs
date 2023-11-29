using System.Net.Http.Json;
using System.Net;
using System.Text.Json;

namespace SalesClient.Client.Security;

public class UserInfoService(HttpClient client) : IUserInfoService
{
    public async Task<UserInfo> GetUserInfo()
    {
        try
        {
            var response = await client.GetAsync("client/user");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return new UserInfo(false, new List<KeyValuePair<string, string>>());
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }

            return await response.Content.ReadFromJsonAsync<UserInfo>(new JsonSerializerOptions(JsonSerializerDefaults.Web));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return null;
    }
}