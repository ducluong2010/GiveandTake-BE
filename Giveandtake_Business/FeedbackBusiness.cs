using GiveandTake_Repo.DTOs.Feedback;
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
    public class FeedbackBusiness
    {
        private readonly UnitOfWork _unitOfWork;

        public FeedbackBusiness()
        {
            _unitOfWork = new UnitOfWork();
        }

        public async Task<IGiveandtakeResult> GetAllFeedbacks(int page = 1, int pageSize = 8)
        {
            var feedbackRepository = _unitOfWork.GetRepository<Feedback>();
            var accountRepository = _unitOfWork.GetRepository<Account>();

            var allAccounts = await accountRepository.GetAllAsync();
            var accountDict = allAccounts.ToDictionary(a => a.AccountId, a => a.FullName);

            var allFeedbacks = await feedbackRepository.GetListAsync(
                predicate: f => true,
                selector: f => new FeedbackDTO
                {
                    FeedbackId = f.FeedbackId,
                    AccountId = f.AccountId,
                    AccountName = f.Account.FullName,
                    DonationId = f.DonationId,
                    DonationName = f.Donation.Name,
                    Rating = f.Rating,
                    Content = f.Content,
                    CreatedDate = f.CreatedDate,
                    FeedbackMediaUrls = f.FeedbackMedia.Select(m => m.MediaUrl).ToList()
                },
                include: source => source
                    .Include(f => f.Account)
                    .Include(f => f.Donation)
                    .Include(f => f.FeedbackMedia)
            );

            int totalItems = allFeedbacks.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (page > totalPages) page = totalPages;
            if (totalItems == 0)
            {
                return new GiveandtakeResult(new PaginatedResult<FeedbackDTO>
                {
                    Items = new List<FeedbackDTO>(),
                    TotalItems = totalItems,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = totalPages
                });
            }

            var paginatedFeedbacks = allFeedbacks
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paginatedResult = new PaginatedResult<FeedbackDTO>
            {
                Items = paginatedFeedbacks,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return new GiveandtakeResult(paginatedResult);
        }
        public async Task<IGiveandtakeResult> GetFeedbackById(int feedbackId)
        {
            var feedbackRepository = _unitOfWork.GetRepository<Feedback>();
            var accountRepository = _unitOfWork.GetRepository<Account>();

            // Fetch the feedback with related entities
            var feedback = await feedbackRepository.SingleOrDefaultAsync(
                predicate: f => f.FeedbackId == feedbackId,
                include: source => source
                    .Include(f => f.Account)
                    .Include(f => f.Donation)
                    .Include(f => f.FeedbackMedia)
            );

            if (feedback == null)
            {
                return new GiveandtakeResult(404, "Feedback not found");
            }

            var feedbackDTO = new FeedbackDTO
            {
                FeedbackId = feedback.FeedbackId,
                AccountId = feedback.AccountId,
                AccountName = feedback.Account?.FullName,
                DonationId = feedback.DonationId,
                DonationName = feedback.Donation?.Name,
                Rating = feedback.Rating,
                Content = feedback.Content,
                CreatedDate = feedback.CreatedDate,
                FeedbackMediaUrls = feedback.FeedbackMedia?.Select(m => m.MediaUrl).ToList() ?? new List<string>()
            };

            return new GiveandtakeResult(feedbackDTO);
        }
        public async Task<IGiveandtakeResult> CreateFeedback(CreateFeedbackDTO createFeedbackDto)
        {
            var account = await _unitOfWork.GetRepository<Account>()
                .FirstOrDefaultAsync(a => a.AccountId == createFeedbackDto.AccountId);
            if (account == null)
            {
                return new GiveandtakeResult(-1, "Account not found");
            }

            var donation = await _unitOfWork.GetRepository<Donation>()
                .FirstOrDefaultAsync(d => d.DonationId == createFeedbackDto.DonationId);
            if (donation == null)
            {
                return new GiveandtakeResult(-1, "Donation not found");
            }

            var newFeedback = new Feedback
            {
                AccountId = createFeedbackDto.AccountId,
                DonationId = createFeedbackDto.DonationId,
                Rating = createFeedbackDto.Rating,
                Content = createFeedbackDto.Content,
                CreatedDate = createFeedbackDto.CreateTime ?? DateTime.Now,
                FeedbackMedia = createFeedbackDto.FeedbackMediaUrls?.Select(url => new FeedbackMedium
                {
                    MediaUrl = url
                }).ToList() ?? new List<FeedbackMedium>()
            };

            await _unitOfWork.GetRepository<Feedback>().InsertAsync(newFeedback);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccessful)
            {
                return new GiveandtakeResult(-1, "Create feedback unsuccessfully");
            }

            return new GiveandtakeResult(1, "Feedback created successfully");
        }
        public async Task<IGiveandtakeResult> UpdateFeedback(int id, UpdateFeedbackDTO feedbackInfo)
        {
            var feedbackRepository = _unitOfWork.GetRepository<Feedback>();
            var existingFeedback = await feedbackRepository.SingleOrDefaultAsync(
                predicate: f => f.FeedbackId == id
            );

            if (existingFeedback == null)
            {
                return new GiveandtakeResult(-1, "Feedback not found");
            }

            if (feedbackInfo.Rating.HasValue)
                existingFeedback.Rating = feedbackInfo.Rating.Value;

            if (!string.IsNullOrEmpty(feedbackInfo.Content))
                existingFeedback.Content = feedbackInfo.Content;

            if (feedbackInfo.CreateTime.HasValue)
                existingFeedback.CreatedDate = feedbackInfo.CreateTime.Value;

            if (feedbackInfo.FeedbackMediaUrls != null && feedbackInfo.FeedbackMediaUrls.Any())
            {
                await UpdateFeedbackMedia(existingFeedback.FeedbackId, feedbackInfo.FeedbackMediaUrls);
            }

            feedbackRepository.UpdateAsync(existingFeedback);
            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult(1, "Feedback updated successfully");
        }
        private async Task UpdateFeedbackMedia(int feedbackId, IEnumerable<string> feedbackMediaUrls)
        {
            var feedbackMediaRepository = _unitOfWork.GetRepository<FeedbackMedium>();

            var existingMedia = await feedbackMediaRepository.GetListAsync(
                predicate: fm => fm.FeedbackId == feedbackId
            );

            if (existingMedia != null && existingMedia.Any())
            {
                foreach (var media in existingMedia)
                {
                    feedbackMediaRepository.DeleteAsync(media);
                }
            }
            foreach (var mediaUrl in feedbackMediaUrls)
            {
                var newFeedbackMedia = new FeedbackMedium
                {
                    FeedbackId = feedbackId,
                    MediaUrl = mediaUrl
                };
                await feedbackMediaRepository.InsertAsync(newFeedbackMedia);
            }
            await _unitOfWork.CommitAsync();
        }
        public async Task<IGiveandtakeResult> DeleteFeedback(int id)
        {
            var feedbackRepository = _unitOfWork.GetRepository<Feedback>();
            var feedbackMediaRepository = _unitOfWork.GetRepository<FeedbackMedium>();

            var existingFeedback = await feedbackRepository.SingleOrDefaultAsync(
                predicate: f => f.FeedbackId == id
            );

            if (existingFeedback == null)
            {
                return new GiveandtakeResult(-1, "Feedback not found");
            }

            var existingMedia = await feedbackMediaRepository.GetListAsync(
                predicate: fm => fm.FeedbackId == id
            );

            if (existingMedia != null && existingMedia.Any())
            {
                foreach (var media in existingMedia)
                {
                    feedbackMediaRepository.DeleteAsync(media);
                }
            }

            feedbackRepository.DeleteAsync(existingFeedback);
            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult(1, "Feedback deleted successfully");
        }
    }
}