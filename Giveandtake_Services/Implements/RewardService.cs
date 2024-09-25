using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Reward;
using Giveandtake_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Implements
{
    public class RewardService : IRewardService
    {
        private readonly RewardBusiness _rewardBusiness;
        public RewardService()
        {
            _rewardBusiness = new RewardBusiness();
        }

        public Task<IGiveandtakeResult> CreateReward(RewardDTO rewardInfo)
            => _rewardBusiness.CreateReward(rewardInfo);

        public Task<IGiveandtakeResult> DeleteReward(int id)
            => _rewardBusiness.DeleteReward(id);

        public Task<IGiveandtakeResult> GetAllRewards()
            => _rewardBusiness.GetAllRewards();

        public Task<IGiveandtakeResult> GetRewardById(int id)
            => _rewardBusiness.GetRewardById(id);

        public Task<IGiveandtakeResult> UpdateReward(int id, RewardDTO rewardInfo)
            => _rewardBusiness.UpdateReward(id, rewardInfo);

        public Task<IGiveandtakeResult> ChangeRewardStatus(int id, string status)
            => _rewardBusiness.ChangeRewardStatus(id, status);
    }
}
