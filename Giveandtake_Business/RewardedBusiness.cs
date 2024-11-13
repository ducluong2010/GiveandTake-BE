using GiveandTake_Repo.DTOs.Reward;
using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using Microsoft.EntityFrameworkCore;
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
                    ClaimedAt = x.ClaimedAt,
                    // Thêm thông tin từ bảng Reward
                    RewardName = x.Reward.RewardName,
                    ImageUrl = x.Reward.ImageUrl
                },
                include: r => r.Include(rewarded => rewarded.Reward)); // Thực hiện include bảng Reward
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
                                          ClaimedAt = x.ClaimedAt,
                                          // Thêm thông tin từ bảng Reward
                                          RewardName = x.Reward.RewardName,
                                          ImageUrl = x.Reward.ImageUrl
                                      },
                                      include: r => r.Include(rewarded => rewarded.Reward)); // Thực hiện include bảng Reward
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
                                  ClaimedAt = x.ClaimedAt,
                                  // Thêm thông tin từ bảng Reward
                                  RewardName = x.Reward.RewardName,
                                  ImageUrl = x.Reward.ImageUrl
                              },
                              include: r => r.Include(rewarded => rewarded.Reward)); // Thực hiện include bảng Reward
            return new GiveandtakeResult(rewarded);
        }


        // Claim reward
        // Claim reward
        public async Task<IGiveandtakeResult> ClaimReward(RewardedDTO rewardedInfo)
        {
            GiveandtakeResult result = new GiveandtakeResult();

            // Lấy thông tin phần thưởng
            var reward = await _unitOfWork.GetRepository<Reward>()
                .SingleOrDefaultAsync(predicate: r => r.RewardId == rewardedInfo.RewardId);

            if (reward == null)
            {
                result.Status = -1;
                result.Message = "Reward not found";
                return result;
            }

            // Lấy thông tin tài khoản
            var customer = await _unitOfWork.GetRepository<Account>()
                .SingleOrDefaultAsync(predicate: c => c.AccountId == rewardedInfo.AccountId);

            if (customer == null)
            {
                result.Status = -1;
                result.Message = "Account not found";
                return result;
            }

            // Kiểm tra điều kiện IsPremium
            if ((bool)reward.IsPremium && !(bool)customer.IsPremium)
            {
                result.Status = -1;
                result.Message = "This reward is available for premium accounts only.";
                return result;
            }

            // Kiểm tra điểm của tài khoản
            if (customer.Point < reward.Point)
            {
                result.Status = -1;
                result.Message = "Insufficient points to claim reward";
                return result;
            }

            // Thực hiện trừ điểm và cập nhật số lượng phần thưởng
            customer.Point -= reward.Point;
            _unitOfWork.GetRepository<Account>().UpdateAsync(customer);

            reward.Quantity -= 1;
            _unitOfWork.GetRepository<Reward>().UpdateAsync(reward);

            // Tạo thông tin phần thưởng đã nhận
            Rewarded rewarded = new Rewarded
            {
                RewardId = rewardedInfo.RewardId,
                AccountId = rewardedInfo.AccountId,
                Status = "Success",
                ClaimedAt = DateTime.Now
            };

            await _unitOfWork.GetRepository<Rewarded>().InsertAsync(rewarded);
            bool status = await _unitOfWork.CommitAsync() > 0;

            if (status)
            {
                // Trả về thông tin phần thưởng đã nhận
                var rewardedDto = new RewardedDTO
                {
                    RewardId = rewarded.RewardId,
                    AccountId = rewarded.AccountId,
                    Status = rewarded.Status,
                    ClaimedAt = rewarded.ClaimedAt,
                    RewardName = reward.RewardName,
                    ImageUrl = reward.ImageUrl
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
