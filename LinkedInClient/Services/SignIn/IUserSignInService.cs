using LinkedInClient.Models.SignIn;
using System.Threading.Tasks;

namespace LinkedInClient.Services.SignIn
{
    public interface IUserSignInService
    {
        Task<LinkedInBasicUserProfileData> GetUserBasicProfileDataAsync(string accessToken);
    }
}