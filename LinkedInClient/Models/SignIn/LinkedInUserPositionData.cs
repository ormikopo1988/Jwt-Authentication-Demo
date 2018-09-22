using Newtonsoft.Json;

namespace LinkedInClient.Models.SignIn
{
    public class LinkedInUserPositionData
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("startDate")]
        public LinkedInDateData StartDate { get; set; }

        [JsonProperty("endDate")]
        public LinkedInDateData EndDate { get; set; }

        [JsonProperty("isCurrent")]
        public bool IsCurrent { get; set; }

        [JsonProperty("location")]
        public LinkedInLocationData Location { get; set; }

        [JsonProperty("company")]
        public LinkedInPositionCompanyData Company { get; set; }
    }
}