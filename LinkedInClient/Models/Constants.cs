namespace LinkedInClient.Models
{
    public static class LinkedInEndpoints
    {
        public const string AccessTokenUri = "https://www.linkedin.com/oauth/v2/accessToken";

        public const string UserBasicProfileDataUri = "https://api.linkedin.com/v1/people/~:(id,first-name,last-name,formatted-name,headline,location,industry,summary,specialties,positions,public-profile-url,picture-url,email-address)";

    }
}