using LinkedInClient.Models.SignIn;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using JwtAuthenticationCoreApi.Core.Models.Entities;
using JwtAuthenticationCoreApi.Persistence;
using JwtAuthenticationCoreApi.Persistence.Repositories;

namespace JwtAuthenticationCoreApi.Core.Repositories
{
    public class JwtAuthUserRepository : GenericRepository<JwtAuthUser>, IJwtAuthUserRepository
    {
        private readonly ApplicationDbContext _context;

        public JwtAuthUserRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<JwtAuthUser> GetJwtAuthUserWithIdentity(string userIdClaimValue)
        {
            return await _context
                            .JwtAuthUsers
                            .Include(c => c.Identity)
                            .SingleAsync(c => c.Identity.Id == userIdClaimValue);
        }

        public async Task SaveLinkedInUserBasicProfileData(
            string identityId, 
            string accessToken, 
            LinkedInBasicUserProfileData userInfo
        )
        {
            await _context.JwtAuthUsers.AddAsync(
                    new JwtAuthUser
                    {
                        IdentityId = identityId,
                        Location = (userInfo.Location != null) ? userInfo.Location.Name : string.Empty,
                        CurrentPosition =
                            (userInfo.Positions != null && userInfo.Positions.Total > 0) ?
                            userInfo.Positions.Values
                                .Where(p => p.IsCurrent == true)
                                .Select(p =>
                                {
                                    return $"{p.Title} at {p.Company.Name}";
                                })
                                .SingleOrDefault()
                            :
                            string.Empty,
                        Headline = userInfo.Headline,
                        PublicProfileUrl = userInfo.PublicProfileUrl,
                        Summary = userInfo.Summary
                    }
                );
        }
    }
}