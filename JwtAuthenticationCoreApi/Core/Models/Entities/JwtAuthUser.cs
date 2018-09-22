namespace JwtAuthenticationCoreApi.Core.Models.Entities
{
    public class JwtAuthUser
    {
        public int Id { get; set; }

        public string Location { get; set; }

        public string Headline { get; set; }

        public string PublicProfileUrl { get; set; }

        public string Summary { get; set; }

        public string CurrentPosition { get; set; }

        public string IdentityId { get; set; }

        public ApplicationUser Identity { get; set; }  // navigation property
    }
}