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
    }
}