using LinkedInClient.Models.SignIn;
using System.Threading.Tasks;
using JwtAuthenticationCoreApi.Core.Models.Entities;

namespace JwtAuthenticationCoreApi.Persistence.Repositories
{
    public interface IJwtAuthUserRepository : IGenericRepository<JwtAuthUser>
    {
        Task<JwtAuthUser> GetJwtAuthUserWithIdentity(string userIdClaimValue);

        Task SaveLinkedInUserBasicProfileData(string identityId, string accessToken, LinkedInBasicUserProfileData userInfo);
    }
}