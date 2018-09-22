using LinkedInClient.Models.OAuth;
using LinkedInClient.Models.SignIn;
using LinkedInClient.Services.OAuth;
using LinkedInClient.Services.SignIn;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using JwtAuthenticationCoreApi.Auth;
using JwtAuthenticationCoreApi.Helpers;
using JwtAuthenticationCoreApi.Core.Models;
using JwtAuthenticationCoreApi.Core.Models.Entities;
using JwtAuthenticationCoreApi.Core.ViewModels;
using JwtAuthenticationCoreApi.Core;
using Microsoft.Extensions.Logging;

namespace JwtAuthenticationCoreApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ExternalAuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly LinkedInOAuthSettings _linkedInAuthSettings;
        private readonly IOAuthService _oAuthService;
        private readonly IUserSignInService _userSignInService;
        private readonly ILogger<ExternalAuthController> _logger;

        public ExternalAuthController(
            UserManager<ApplicationUser> userManager, 
            IUnitOfWork unitOfWork, 
            IJwtFactory jwtFactory, 
            IOptions<JwtIssuerOptions> jwtOptions, 
            IOptions<LinkedInOAuthSettings> linkedInAuthSettingsAccessor,
            IOAuthService oAuthService,
            IUserSignInService userSignInService,
            ILogger<ExternalAuthController> logger
        )
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
            _linkedInAuthSettings = linkedInAuthSettingsAccessor.Value;
            _oAuthService = oAuthService;
            _userSignInService = userSignInService;
            _logger = logger;
        }

        // GET api/externalauth/callback
        [HttpGet]
        public async Task<ActionResult> Callback()
        {
            string redirectUrl = "http://localhost:4200/linkedin-auth.html";

            try
            {
                var authCode = HttpContext.Request.Query["code"].ToString();
                var state = HttpContext.Request.Query["state"].ToString();
                
                var accessTokenResponse = await _oAuthService.GetAccessTokenAsync(
                    authCode,
                    "https://localhost:44364/api/externalauth/callback", 
                    new LinkedInOAuthSettings
                    {
                        ClientId = _linkedInAuthSettings.ClientId,
                        ClientSecret = _linkedInAuthSettings.ClientSecret
                    }
                );

                if(accessTokenResponse != null && accessTokenResponse.AccessToken != null)
                {
                    redirectUrl += $"?access_token={accessTokenResponse.AccessToken}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in ExternalAuthController:Callback action.");
            }

            return Redirect(redirectUrl);
        }

        // POST api/externalauth/linkedin
        [HttpPost]
        public async Task<IActionResult> LinkedIn([FromBody]LinkedInAuthViewModel model)
        {   
            // 1. Request user data from LinkedIn
            var userInfo = await _userSignInService.GetUserBasicProfileDataAsync(model.AccessToken);

            if (userInfo == null)
            {
                return BadRequest(Errors.AddErrorToModelState("login_failure", "Invalid or expired LinkedIn token.", ModelState));
            }

            // 2. ready to create the local user account (if necessary) and jwt
            var user = await _userManager.FindByNameAsync(userInfo.Email);

            if (user == null)
            {
                var appUser = new ApplicationUser
                {
                    FirstName = userInfo.FirstName,
                    LastName = userInfo.LastName,
                    LinkedInId = userInfo.Id,
                    Email = userInfo.Email,
                    UserName = userInfo.Email,
                    PictureUrl = userInfo.PictureUrl
                };

                var result = await _userManager.CreateAsync(appUser, Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8));

                if (!result.Succeeded)
                {
                    return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));
                }

                await _unitOfWork.JwtAuthUsersRepository.SaveLinkedInUserBasicProfileData(
                    appUser.Id,
                    model.AccessToken,
                    userInfo
                );

                await _unitOfWork.CompleteAsync();
            }
            else
            {
                // If the user has selected LinkedIn SignIn/SignUp and arrives here
                // that means that the user has already registered with local account.
                // Update LinkedInId and PictureUrl in the Db if needed. 
                // In case the user registered with local account and now signed 
                // in with linkedIn with same email address
                if (this._linkedInDataNeedsUpdate(user, userInfo))
                {
                    user.LinkedInId = userInfo.Id;
                    user.PictureUrl = userInfo.PictureUrl;

                    await _userManager.UpdateAsync(user);
                }
            }

            // Try find the local user again if necessary and generate the jwt for him/her
            var localUser = await _userManager.FindByNameAsync(userInfo.Email);

            if (localUser == null)
            {
                return BadRequest(Errors.AddErrorToModelState("login_failure", "Failed to create local user account.", ModelState));
            }

            var jwt = await Tokens.GenerateJwt(_jwtFactory.GenerateClaimsIdentity(localUser.UserName, localUser.Id), _jwtFactory, localUser.UserName, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });

            return new OkObjectResult(jwt);
        }

        #region Private Helper Methods

        private bool _linkedInDataNeedsUpdate(ApplicationUser localUser, LinkedInBasicUserProfileData user)
        {
            return 
                (string.IsNullOrEmpty(localUser.LinkedInId)) || 
                (!string.IsNullOrEmpty(localUser.LinkedInId) && localUser.LinkedInId != user.Id) ||
                (string.IsNullOrEmpty(localUser.PictureUrl)) || 
                (!string.IsNullOrEmpty(localUser.PictureUrl) && localUser.PictureUrl != user.PictureUrl);
        }
        
        #endregion
    }
}