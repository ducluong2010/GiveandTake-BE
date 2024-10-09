using GiveandTake_Repo.DTOs.Reward;
using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Business
{
    public class RewardBusiness
    {
        private UnitOfWork _unitOfWork;
        public RewardBusiness()
        {
            _unitOfWork = new UnitOfWork();
        }

        #region Reward
        // Get all reward
        public async Task<IGiveandtakeResult> GetAllRewards()
        {
            var rewardList = await _unitOfWork.GetRepository<Reward>()
                .GetListAsync(selector: x => new GetRewardDTO
                {
                    RewardId = x.RewardId,
                    AccountId = x.AccountId,
                    RewardName = x.RewardName,
                    Description = x.Description,
                    ImageUrl = x.ImageUrl,
                    Point = x.Point,
                    Quantity = x.Quantity,
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.UpdatedDate,
                    Status = x.Status
                });
            return new GiveandtakeResult(rewardList);
        }

        // Get reward by id
        public async Task<IGiveandtakeResult> GetRewardById(int rewardId)
        {
            var reward = await _unitOfWork.GetRepository<Reward>()
                .SingleOrDefaultAsync(predicate: c => c.RewardId == rewardId,
                                      selector: x => new GetRewardDTO
                                      {
                                          RewardId = x.RewardId,
                                          AccountId = x.AccountId,
                                          RewardName = x.RewardName,
                                          Description = x.Description,
                                          ImageUrl = x.ImageUrl,
                                          Point = x.Point,
                                          Quantity = x.Quantity,
                                          CreatedDate = x.CreatedDate,
                                          UpdatedDate = x.UpdatedDate,
                                          Status = x.Status
                                      });
            return new GiveandtakeResult(reward);
        }

        // Update reward information
        public async Task<IGiveandtakeResult> UpdateReward(int id, RewardDTO rewardInfo)
        {
            Reward reward = await _unitOfWork.GetRepository<Reward>()
                .SingleOrDefaultAsync(predicate: c => c.RewardId == id);
            if (reward == null)
            {
                return new GiveandtakeResult(-1, "Reward not found");
            }
            else
            {
                // Validate point and quantity
                if (rewardInfo.Point < 0 || rewardInfo.Quantity < 0)
                {
                    return new GiveandtakeResult(-1, "Point and Quantity must be greater than 0");
                }

                // Update status to "Claimed" if quantity is 0
                if (rewardInfo.Quantity == 0)
                {
                    rewardInfo.Status = "Claimed";
                }

                reward.RewardName = String.IsNullOrEmpty(rewardInfo.RewardName) ? reward.RewardName : rewardInfo.RewardName;
                reward.Description = String.IsNullOrEmpty(rewardInfo.Description) ? reward.Description : rewardInfo.Description;
                reward.ImageUrl = String.IsNullOrEmpty(rewardInfo.ImageUrl) ? reward.ImageUrl : rewardInfo.ImageUrl;
                reward.Point = rewardInfo.Point > 0 ? rewardInfo.Point : reward.Point;
                reward.Quantity = rewardInfo.Quantity >= 0 ? rewardInfo.Quantity : reward.Quantity;
                reward.UpdatedDate = DateTime.Now;
                reward.Status = String.IsNullOrEmpty(rewardInfo.Status) ? reward.Status : rewardInfo.Status;

                _unitOfWork.GetRepository<Reward>().UpdateAsync(reward);
                await _unitOfWork.CommitAsync();
            }
            return new GiveandtakeResult(1, "Reward updated successfully");
        }

        // Add new reward
        public async Task<IGiveandtakeResult> CreateReward(int accountId, RewardDTO rewardInfo)
        {
            GiveandtakeResult result = new GiveandtakeResult();
            // Validate point and quantity
            if (rewardInfo.Point < 0 || rewardInfo.Quantity < 0)
            {
                return new GiveandtakeResult(-1, "Point and Quantity must be greater than 0");
            }

            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate:
                c => c.AccountId == accountId);
            if (account == null)
            {
                return new GiveandtakeResult(-1, "Account not found");
            }

            // Update status to "Claimed" if quantity is 0
            if (rewardInfo.Quantity == 0)
            {
                rewardInfo.Status = "Claimed";
            }

            Reward newReward = new Reward
            {
                AccountId = accountId,
                RewardName = rewardInfo.RewardName,
                Description = rewardInfo.Description,
                ImageUrl = rewardInfo.ImageUrl,
                Point = rewardInfo.Point,
                Quantity = rewardInfo.Quantity,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                Status = rewardInfo.Status
            };
            await _unitOfWork.GetRepository<Reward>().InsertAsync(newReward);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (isSuccessful)
            {
                result = new GiveandtakeResult(1, "Create Successful");
            }
            else
            {
                result.Status = -1;
                result.Message = "Create Unsuccessfully";
            }
            return result;
        }

        // Delete reward
        public async Task<IGiveandtakeResult> DeleteReward(int id)
        {
            Reward reward = await _unitOfWork.GetRepository<Reward>()
                .SingleOrDefaultAsync(predicate: c => c.RewardId == id);
            if (reward == null)
            {
                return new GiveandtakeResult(-1, "Reward not found");
            }
            _unitOfWork.GetRepository<Reward>().DeleteAsync(reward);
            await _unitOfWork.CommitAsync();
            return new GiveandtakeResult(1, "Reward deleted successfully");
        }

        // Change reward status
        public async Task<IGiveandtakeResult> ChangeRewardStatus(int id, string newStatus)
        {
            if (newStatus.ToLower() != "inactive" && newStatus.ToLower() != "active" && newStatus.ToLower() != "claimed" && newStatus.ToLower() != "premium")
            {
                return new GiveandtakeResult(-4, "Invalid status");
            }


            Reward reward = await _unitOfWork.GetRepository<Reward>()
                .SingleOrDefaultAsync(predicate: c => c.RewardId == id);
            if (reward == null)
            {
                return new GiveandtakeResult(-1, "Reward not found");
            }
            else
            {
                reward.Status = newStatus;
                reward.UpdatedDate = DateTime.Now;

                _unitOfWork.GetRepository<Reward>().UpdateAsync(reward);
                await _unitOfWork.CommitAsync();
            }

            return new GiveandtakeResult(1, "Reward status updated successfully");
        }

        #endregion
    }
}