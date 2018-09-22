using Newtonsoft.Json;

namespace LinkedInClient.Models.SignIn
{
    public class LinkedInDateData
    {
        [JsonProperty("month")]
        public int Month { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }
    }
}