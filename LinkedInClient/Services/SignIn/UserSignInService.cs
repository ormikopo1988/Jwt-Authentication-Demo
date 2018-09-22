using LinkedInClient.Models;
using LinkedInClient.Models.SignIn;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LinkedInClient.Services.SignIn
{
    public class UserSignInService : IUserSignInService
    {
        internal IWebApiClientService webApiClientInstance = WebApiClientFactory.CreateWebApiClientService();

        public async Task<LinkedInBasicUserProfileData> GetUserBasicProfileDataAsync(string accessToken)
        {
            Dictionary<string, string> requestHeaders = new Dictionary<string, string>();

            requestHeaders.Add(HttpRequestHeader.Authorization.ToString(), $"Bearer {accessToken}");
            requestHeaders.Add("x-li-format", "json");

            var userInfoResponse = await webApiClientInstance.CustomGetAsync(new Uri(LinkedInEndpoints.UserBasicProfileDataUri), requestHeaders);

            if (!userInfoResponse.IsSuccessStatusCode)
            {
                return null;
            }

            var userInfoStr = await userInfoResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<LinkedInBasicUserProfileData>(userInfoStr);
        }
    }
}