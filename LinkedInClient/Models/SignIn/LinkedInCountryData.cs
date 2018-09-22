using Newtonsoft.Json;

namespace LinkedInClient.Models.SignIn
{
    public class LinkedInCountryData
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}