using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Reward;
using GiveandTake_Repo.Models;
using Giveandtake_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Implements
{
    public class RewardedService : IRewardedService
    {
        public Task<IGiveandtakeResult> GetAllRewarded()
            => new RewardedBusiness().GetAllRewarded();

        public Task<IGiveandtakeResult> GetRewardedByAccountId(int accountId)
            => new RewardedBusiness().GetRewardedByAccountId(accountId);

        public Task<IGiveandtakeResult> GetRewardedById(int id)
            => new RewardedBusiness().GetRewardedById(id);

        public Task<IGiveandtakeResult> ClaimReward(RewardedDTO rewardedInfo)
            => new RewardedBusiness().ClaimReward(rewardedInfo);

    }
}
