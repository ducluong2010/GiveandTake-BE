using GiveandTake_API.Constants;
using GiveandTake_Repo.DTOs.Account;
using GiveandTake_Repo.DTOs.Reward;
using GiveandTake_Repo.Models;
using Giveandtake_Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static GiveandTake_API.Constants.ApiEndPointConstant;

namespace GiveandTake_API.Controllers
{
    [ApiController]
    public class RewardedController : ControllerBase
    {
        private readonly IRewardedService _rewardedService;
        private readonly IAccountService _accountService;
        private readonly IRewardService _rewardService;

        public RewardedController(IRewardedService rewardedService, IAccountService accountService, IRewardService rewardService)
        {
            _rewardedService = rewardedService;
            _accountService = accountService;
            _rewardService = rewardService;
        }

        [HttpGet(ApiEndPointConstant.Rewarded.RewardedEndPoint)]
        [SwaggerOperation(Summary = "Get all rewarded items")]
        public async Task<IActionResult> GetAllRewarded()
        {
            var result = await _rewardedService.GetAllRewarded();
            return Ok(result);
        }

        [HttpGet(ApiEndPointConstant.Rewarded.RewardedByIdEndPoint)]
        [SwaggerOperation(Summary = "Get rewarded item by id")]
        public async Task<IActionResult> GetRewardedById(int id)
        {
            var result = await _rewardedService.GetRewardedById(id);
            return Ok(result);
        }

        [HttpGet(ApiEndPointConstant.Rewarded.RewardedByAccountEndPoint)]
        [SwaggerOperation(Summary = "Get rewarded items by account id")]
        public async Task<IActionResult> GetRewardedByAccountId(int accountId)
        {
            var result = await _rewardedService.GetRewardedByAccountId(accountId);
            return Ok(result);
        }

        
    }
}
