using Newtonsoft.Json;

namespace LinkedInClient.Models.OAuth
{
    public class LinkedInAccessToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public string ExpiresIn { get; set; }
    }
}