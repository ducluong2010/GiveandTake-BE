using GiveandTake_API.Constants;
using GiveandTake_Repo.DTOs.Account;
using Giveandtake_Services.Implements;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GiveandTake_API.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountService _accountService;

        public AccountController()
        {
            _accountService = new AccountService();
        }

        [HttpGet(ApiEndPointConstant.Account.AccountsEndpoint)]
        [SwaggerOperation(Summary = "Get all Accounts with pagination")]
        public async Task<IActionResult> GetAllAccount([FromQuery] int page = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _accountService.GetAllAccount(page, pageSize);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpGet(ApiEndPointConstant.Account.AccountEndpoint)]
        [SwaggerOperation(Summary = "Get Account by its id")]
        public async Task<IActionResult> GetAccountInfo(int id)
        {
            var response = await _accountService.GetAccountInfo(id);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }

        [HttpGet(ApiEndPointConstant.Account.EmailAccountsEndpoint)]
        [SwaggerOperation(Summary = "Get Account by its email")]
        public async Task<IActionResult> GetAccountByEmail(string email)
        {
            var response = await _accountService.GetAccountInfoByEmail(email);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }

        [HttpPost(ApiEndPointConstant.Account.AccountsEndpoint)]
        [SwaggerOperation(Summary = "Create a new Account")]
        public async Task<IActionResult> CreateAccount(AccountDTO accountDTO)
        {
            var response = await _accountService.CreateAccount(accountDTO);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpPut(ApiEndPointConstant.Account.AccountEndpoint)]
        [SwaggerOperation(Summary = "Update Account Info")]
        public async Task<IActionResult> UpdateAccountInfo(int id, AccountDTO account)
        {
            var response = await _accountService.UpdateAccountInfo(id, account);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpPut(ApiEndPointConstant.Account.PromoteToPremiumEndPoint)]
        [SwaggerOperation(Summary = "Promote Account to Premium")]
        public async Task<IActionResult> PromoteToPremium(int id)
        {
            var response = await _accountService.PromoteToPremium(id);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }


        [HttpDelete(ApiEndPointConstant.Account.AccountEndpoint)]
        [SwaggerOperation(Summary = "Delete Account")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var response = await _accountService.DeleteAccount(id);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpPut(ApiEndPointConstant.Account.BanAccountEndPoint)]
        [SwaggerOperation(Summary = "Ban Account")]
        public async Task<IActionResult> BanAccount(int id)
        {
            var response = await _accountService.BanAccount(id);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpPut(ApiEndPointConstant.Account.UnbanAccountEndPoint)]
        [SwaggerOperation(Summary = "Unban Account")]
        public async Task<IActionResult> UnbanAccount(int id)
        {
            var response = await _accountService.UnbanAccount(id);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpPut(ApiEndPointConstant.Account.ChangePasswordEndPoint)]
        [SwaggerOperation(Summary = "Change Account Password")]
        public async Task<IActionResult> ChangePassword(int id, string oldPassword, string newPassword)
        {
            var response = await _accountService.ChangePassword(id, oldPassword, newPassword);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }
    }
}
