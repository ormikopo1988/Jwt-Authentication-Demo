using LinkedInClient.Models.OAuth;
using System.Threading.Tasks;

namespace LinkedInClient.Services.OAuth
{
    public interface IOAuthService
    {
        Task<LinkedInAccessToken> GetAccessTokenAsync(string authorizationCode, string redirectUri, LinkedInOAuthSettings clientCredentials);
    }
}