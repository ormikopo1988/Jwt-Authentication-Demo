using LinkedInClient.Models;
using LinkedInClient.Models.OAuth;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace LinkedInClient.Services.OAuth
{
    public class OAuthService : IOAuthService
    {
        internal IWebApiClientService webApiClientInstance = WebApiClientFactory.CreateWebApiClientService();

        public async Task<LinkedInAccessToken> GetAccessTokenAsync(string authorizationCode, string redirectUri, LinkedInOAuthSettings clientCredentials)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("grant_type", "authorization_code");
            dict.Add("code", authorizationCode);
            dict.Add("redirect_uri", redirectUri);
            dict.Add("client_id", clientCredentials.ClientId);
            dict.Add("client_secret", clientCredentials.ClientSecret);

            var response = await webApiClientInstance.PostAsyncFacade(LinkedInEndpoints.AccessTokenUri, new FormUrlEncodedContent(dict));

            if (response.IsSuccessStatusCode)
            {
                var accessTokenResponseStr = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<LinkedInAccessToken>(accessTokenResponseStr);
            }

            return null;
        }
    }
}