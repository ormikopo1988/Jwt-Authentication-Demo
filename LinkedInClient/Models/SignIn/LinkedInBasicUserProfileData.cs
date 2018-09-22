using Newtonsoft.Json;

namespace LinkedInClient.Models.SignIn
{
    public class LinkedInBasicUserProfileData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("emailAddress")]
        public string Email { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }
        
        [JsonProperty("formattedName")]
        public string FormattedName { get; set; }

        [JsonProperty("headline")]
        public string Headline { get; set; }

        [JsonProperty("location")]
        public LinkedInLocationData Location { get; set; }

        [JsonProperty("industry")]
        public string Industry { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("positions")]
        public LinkedInUserPositions Positions { get; set; }

        [JsonProperty("pictureUrl")]
        public string PictureUrl { get; set; }

        [JsonProperty("publicProfileUrl")]
        public string PublicProfileUrl { get; set; }
    }
}