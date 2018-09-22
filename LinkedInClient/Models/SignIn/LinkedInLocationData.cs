using Newtonsoft.Json;

namespace LinkedInClient.Models.SignIn
{
    public class LinkedInLocationData
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("country")]
        public LinkedInCountryData Country { get; set; }
    }
}