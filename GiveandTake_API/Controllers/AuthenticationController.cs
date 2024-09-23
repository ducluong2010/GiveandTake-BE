using GiveandTake_API.Constants;
using Giveandtake_Services.Implements;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GiveandTake_API.Controllers
{
    [ApiController]
    // Controller for handling authentication-related requests
    public class AuthenticationController : ControllerBase
    {
        private readonly AccountService _accountService;

        public AuthenticationController()
        {
            _accountService = new AccountService();
        }

        // Endpoint for user login
        [HttpPost(ApiEndPointConstant.Authentication.LoginEndpoint)]
        [SwaggerOperation(Summary = "Login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var response = await _accountService.Login(email, password);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

        // Endpoint for user registration
        [HttpPost(ApiEndPointConstant.Authentication.RegisterEndpoint)]
        [SwaggerOperation(Summary = "Register")]
        public async Task<IActionResult> Register(string email, string password)
        {
            var response = await _accountService.Register(email, password);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }
    }
}
