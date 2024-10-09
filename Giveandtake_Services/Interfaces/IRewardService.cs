using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Reward;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Interfaces
{
    public interface IRewardService
    {
        Task<IGiveandtakeResult> GetAllRewards();
        Task<IGiveandtakeResult> GetRewardById(int id);
        Task<IGiveandtakeResult> UpdateReward(int id, RewardDTO rewardInfo);
        Task<IGiveandtakeResult> CreateReward(int accountId, RewardDTO rewardInfo);
        Task<IGiveandtakeResult> DeleteReward(int id);
        Task<IGiveandtakeResult> ChangeRewardStatus(int id, string status);
    }
}
