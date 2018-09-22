using Microsoft.AspNetCore.Identity;

namespace JwtAuthenticationCoreApi.Core.Models.Entities
{
    // Add profile data for application users by adding properties to this class
    public class ApplicationUser : IdentityUser
    {
        // Extended Properties
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string LinkedInId { get; set; }

        public string PictureUrl { get; set; }
    }
}