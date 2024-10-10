using GiveandTake_Repo.DTOs.Reward;
using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Business
{
    public class RewardedBusiness
    {
        private readonly UnitOfWork _unitOfWork;

        public RewardedBusiness()
        {
            _unitOfWork = new UnitOfWork();
        }

        #region Claim Reward

        // Get all claimed rewards
        public async Task<IGiveandtakeResult> GetAllRewarded()
        {
            var rewardedList = await _unitOfWork.GetRepository<Rewarded>()
                .GetListAsync(selector: x => new GetRewardedDTO
                {
                    RewardedId = x.RewardedId,
                    RewardId = x.RewardId,
                    AccountId = x.AccountId,
                    Status = x.Status,
                    ClaimedAt = x.ClaimedAt
                });
            return new GiveandtakeResult(rewardedList);
        }

        // Get claimed rewards by its id
        public async Task<IGiveandtakeResult> GetRewardedById(int rewardedId)
        {
            var rewarded = await _unitOfWork.GetRepository<Rewarded>()
                .SingleOrDefaultAsync(predicate: c => c.RewardedId == rewardedId,
                                      selector: x => new GetRewardedDTO
                                      {
                                          RewardedId = x.RewardedId,
                                          RewardId = x.RewardId,
                                          AccountId = x.AccountId,
                                          Status = x.Status,
                                          ClaimedAt = x.ClaimedAt
                                      });
            return new GiveandtakeResult(rewarded);
        }

        // Get claimed rewards by account id    
        public async Task<IGiveandtakeResult> GetRewardedByAccountId(int accountId)
        {
            var rewarded = await _unitOfWork.GetRepository<Rewarded>()
                .GetListAsync(predicate: c => c.AccountId == accountId,
                              selector: x => new GetRewardedDTO
                              {
                                  RewardedId = x.RewardedId,
                                  RewardId = x.RewardId,
                                  AccountId = x.AccountId,
                                  Status = x.Status,
                                  ClaimedAt = x.ClaimedAt
                              });
            return new GiveandtakeResult(rewarded);
        }


        // Claim reward
         public async Task<IGiveandtakeResult> ClaimReward(Rewarded rewardedInfo)
        {
            Rewarded rewarded = new Rewarded
            {
                RewardId = rewardedInfo.RewardId,
                AccountId = rewardedInfo.AccountId,
                Status = rewardedInfo.Status,
                ClaimedAt = DateTime.Now
            };

            await _unitOfWork.GetRepository<Rewarded>().InsertAsync(rewarded);

            GiveandtakeResult result = new GiveandtakeResult();
            bool status = await _unitOfWork.CommitAsync() > 0;
            if (status) {
                result.Data = rewarded;
                result.Status = 1;
                result.Message = "Claim reward successfully";
            } else {
                result.Status = -1;
                result.Message = "Failed to claim reward";
            }
            return result;
        }

        //public async Task<IGiveandtakeResult> ClaimReward(Account account, Rewarded rewardedInfo)
        //{
        //    var accountEntity = await _unitOfWork.GetRepository<Account>()
        //        .SingleOrDefaultAsync(predicate: c => c.AccountId == account.AccountId);

        //    if (accountEntity == null)
        //    {
        //        return new GiveandtakeResult(-1, "Account not found");
        //    }

        //    if (accountEntity.Point < rewardedInfo.Reward.Point)
        //    {
        //        return new GiveandtakeResult(-1, "Not enough points to redeem the reward");
        //    }

        //    accountEntity.Point -= rewardedInfo.Reward.Point;
        //    rewardedInfo.Reward.Quantity--;

        //    var rewarded = new Rewarded
        //    {
        //        RewardId = rewardedInfo.RewardId,
        //        AccountId = rewardedInfo.AccountId,
        //        Status = rewardedInfo.Status,
        //        CreatedAt = DateTime.Now
        //    };

        //    _unitOfWork.GetRepository<Rewarded>().InsertAsync(rewarded);
        //    _unitOfWork.GetRepository<Account>().UpdateAsync(accountEntity);

        //    bool status = await _unitOfWork.CommitAsync() > 0;

        //    if (status)
        //    {
        //        return new GiveandtakeResult(1, "Claim reward successfully");
        //    }
        //    else
        //    {
        //        return new GiveandtakeResult(-1, "Failed to claim reward");
        //    }
        //}

        #endregion

    }
}
