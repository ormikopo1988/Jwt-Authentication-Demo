using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JwtAuthenticationCoreApi.Core.Models.Entities;
using JwtAuthenticationCoreApi.Core;

namespace JwtAuthenticationCoreApi.Controllers
{
    [ApiController]
    [Authorize(Policy = "ApiUser")]
    [Route("api/[controller]/[action]")]
    public class DashboardController : ControllerBase
    {
        private readonly ClaimsPrincipal _caller;
        private readonly IUnitOfWork _unitOfWork;
        
        public DashboardController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _caller = httpContextAccessor.HttpContext.User;
            _unitOfWork = unitOfWork;
        }

        // GET api/dashboard/home
        [HttpGet]
        public async Task<IActionResult> Home()
        {
            // Retrieve the user info
            // HttpContext.User
            var userId = _caller.Claims.Single(c => c.Type == "id");

            var customer = await _unitOfWork.JwtAuthUsersRepository.GetJwtAuthUserWithIdentity(userId.Value);

            return new OkObjectResult(new
            {
                Message = "This is secure API and user data!",
                customer.Identity.FirstName,
                customer.Identity.LastName,
                customer.Identity.PictureUrl,
                customer.Identity.LinkedInId,
                customer.Location,
                customer.CurrentPosition,
                customer.Headline,
                customer.PublicProfileUrl,
                customer.Summary
            });
        }
    }
}