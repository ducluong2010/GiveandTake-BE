using GiveandTake_API.Constants;
using GiveandTake_Repo.DTOs.Account;
using GiveandTake_Repo.DTOs.Reward;
using GiveandTake_Repo.Models;
using Giveandtake_Services.Implements;
using Giveandtake_Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static GiveandTake_API.Constants.ApiEndPointConstant;

namespace GiveandTake_API.Controllers
{
    [ApiController]
    public class RewardedController : ControllerBase
    {
        private readonly RewardedService _rewardedService;


        public RewardedController()
        {
            _rewardedService = new RewardedService();
        }

        [HttpGet(ApiEndPointConstant.Rewarded.RewardedEndPoint)]
        [SwaggerOperation(Summary = "Get all rewarded items")]
        public async Task<IActionResult> GetAllRewarded()
        {
            var response = await _rewardedService.GetAllRewarded();
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }

        [HttpGet(ApiEndPointConstant.Rewarded.RewardedByIdEndPoint)]
        [SwaggerOperation(Summary = "Get rewarded item by id")]
        public async Task<IActionResult> GetRewardedById(int id)
        {
            var response = await _rewardedService.GetRewardedById(id);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }

        [HttpGet(ApiEndPointConstant.Rewarded.RewardedByAccountEndPoint)]
        [SwaggerOperation(Summary = "Get rewarded items by account id")]
        public async Task<IActionResult> GetRewardedByAccountId(int accountId)
        {
            var response = await _rewardedService.GetRewardedByAccountId(accountId);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }

        [HttpPost(ApiEndPointConstant.Rewarded.RewardedEndPoint)]
        [SwaggerOperation(Summary = "Claim a reward")]
        public async Task<IActionResult> ClaimReward(RewardedDTO rewardedInfo)
        {
            int accountId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId").Value);

            rewardedInfo.AccountId = accountId;

            var response = await _rewardedService.ClaimReward(rewardedInfo);

            if (response.Status >= 0)
            {
                return Ok(response.Data);
            }
            else
            {
                return BadRequest(response.Message); 
            }
        }

    }
}
