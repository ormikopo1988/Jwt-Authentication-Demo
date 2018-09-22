using Newtonsoft.Json;
using System.Collections.Generic;

namespace LinkedInClient.Models.SignIn
{
    public class LinkedInUserPositions
    {
        [JsonProperty("_total")]
        public int Total { get; set; }

        [JsonProperty("values")]
        public List<LinkedInUserPositionData> Values { get; set; }
    }
}