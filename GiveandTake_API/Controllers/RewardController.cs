using GiveandTake_API.Constants;
using GiveandTake_Repo.DTOs.Reward;
using Giveandtake_Services.Implements;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GiveandTake_API.Controllers
{
    [ApiController]
    public class RewardController : ControllerBase
    {
        private readonly RewardService _rewardService;
        public RewardController()
        {
           _rewardService = new RewardService();
        }

        [HttpGet(ApiEndPointConstant.Reward.RewardsEndPoint)]
        [SwaggerOperation(Summary = "Get all Rewards")]
        public async Task<IActionResult> GetAllRewards()
        {
            var response = await _rewardService.GetAllRewards();
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }

        [HttpGet(ApiEndPointConstant.Reward.RewardEndPoint)]
        [SwaggerOperation(Summary = "Get Reward by its id")]
        public async Task<IActionResult> GetRewardInfo(int id)
        {
            var response = await _rewardService.GetRewardById(id);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }

        [HttpPost(ApiEndPointConstant.Reward.RewardsEndPoint)]
        [SwaggerOperation(Summary = "Create a new Reward")]
        public async Task<IActionResult> CreateReward(RewardDTO rewardDTO)
        {
            var response = await _rewardService.CreateReward(rewardDTO);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpPut(ApiEndPointConstant.Reward.RewardEndPoint)]
        [SwaggerOperation(Summary = "Update Reward")]
        public async Task<IActionResult> UpdateRewardInfo(int id, RewardDTO reward)
        {
            var response = await _rewardService.UpdateReward(id, reward);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpDelete(ApiEndPointConstant.Reward.RewardEndPoint)]
        [SwaggerOperation(Summary = "Delete Reward")]
        public async Task<IActionResult> DeleteReward(int id)
        {
            var response = await _rewardService.DeleteReward(id);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpPut(ApiEndPointConstant.Reward.RewardStatusEndPoint)]
        [SwaggerOperation(Summary = "Change Reward Status")]
        public async Task<IActionResult> ChangeRewardStatus(int id, string status)
        {
            var response = await _rewardService.ChangeRewardStatus(id, status);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

    }
}
