using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Reward;
using GiveandTake_Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Interfaces
{
    public interface IRewardedService
    {
        Task<IGiveandtakeResult> GetAllRewarded();
        Task<IGiveandtakeResult> GetRewardedById(int id);
        Task<IGiveandtakeResult> GetRewardedByAccountId(int accountId);
        Task <IGiveandtakeResult> ClainReward(Rewarded rewardedInfo);
        //Task<IGiveandtakeResult> ClaimReward(Account account, Rewarded rewardedInfo);
    }
}
