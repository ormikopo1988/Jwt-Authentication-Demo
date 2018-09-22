using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using JwtAuthenticationCoreApi.Helpers;
using JwtAuthenticationCoreApi.Core.Models.Entities;
using JwtAuthenticationCoreApi.Core.ViewModels;
using JwtAuthenticationCoreApi.Core;

namespace JwtAuthenticationCoreApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountsController(
            UserManager<ApplicationUser> userManager, 
            IUnitOfWork unitOfWork
        )
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        // POST api/accounts
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdentity = new ApplicationUser
            {
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(userIdentity, model.Password);

            if (!result.Succeeded)
            {
                return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));
            }

            var newUser = await _unitOfWork.JwtAuthUsersRepository.InsertAsync(
                new JwtAuthUser
                {
                    IdentityId = userIdentity.Id,
                    Location = model.Location
                }
            );

            await _unitOfWork.CompleteAsync();

            return new OkObjectResult(
                JsonConvert.SerializeObject(
                    new
                    {
                        model.Email,
                        Message = "Account Created"
                    }, 
                    new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented
                    }
                )
            );
        }
    }
}