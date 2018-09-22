using Newtonsoft.Json;

namespace LinkedInClient.Models.SignIn
{
    public class LinkedInPositionCompanyData
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("industry")]
        public string Industry { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}