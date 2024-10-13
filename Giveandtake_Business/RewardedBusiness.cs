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
        public async Task<IGiveandtakeResult> ClaimReward(RewardedDTO rewardedInfo)
        {
            Rewarded rewarded = new Rewarded
            {
                RewardId = rewardedInfo.RewardId,
                AccountId = rewardedInfo.AccountId,
                Status = rewardedInfo.Status,
                ClaimedAt = DateTime.Now
            };

            var reward = await _unitOfWork.GetRepository<Reward>()
                .SingleOrDefaultAsync(predicate: r => r.RewardId == rewardedInfo.RewardId);

            if (reward != null)
            {
                var customer = await _unitOfWork.GetRepository<Account>()
                    .SingleOrDefaultAsync(predicate: c => c.AccountId == rewardedInfo.AccountId);

                if (customer != null && customer.Point >= reward.Point)
                {
                    customer.Point -= reward.Point;
                    _unitOfWork.GetRepository<Account>().UpdateAsync(customer);
                    rewarded.Status = "Success";
                    reward.Quantity -= 1;

                   _unitOfWork.GetRepository<Reward>().UpdateAsync(reward);

                }
                else
                {
                    GiveandtakeResult insufficientPointsResult = new GiveandtakeResult();
                    insufficientPointsResult.Status = -1;
                    insufficientPointsResult.Message = "Insufficient points to claim reward";
                    return insufficientPointsResult;
                }
            }

            await _unitOfWork.GetRepository<Rewarded>().InsertAsync(rewarded);
            bool status = await _unitOfWork.CommitAsync() > 0;

            GiveandtakeResult result = new GiveandtakeResult();
            if (status)
            {
                var rewardedDto = new RewardedDTO
                {
                    RewardId = rewarded.RewardId,
                    AccountId = rewarded.AccountId,
                    Status = rewarded.Status,
                    ClaimedAt = rewarded.ClaimedAt
                };

                result.Data = rewardedDto;
                result.Status = 1;
                result.Message = "Claim reward successfully";
            }
            else
            {
                result.Status = -1;
                result.Message = "Failed to claim reward";
            }

            return result;
        }
        #endregion
    }
}
